// Copyright (C) Tenacom and contributors. Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

#nullable enable

// ---------------------------------------------------------------------------------------------
// Option helpers
// ---------------------------------------------------------------------------------------------

using System.ComponentModel;
using System.Linq;

/*
 * Summary : Tells whether the specified option is present, either as an argument
 *           or as an environment variable.
 * Params  : name              - The option name.
 *           environmentPrefix - An optional prefix for the environment variable name;
 *                               for example, camelCasedOption (prefix = "MYAPP_") -> MYAPP_CAMEL_CASED_OPTION
 * Returns : If an argument with the specified name is present, true;
 *           if an environment variable with the specified name (converted according to environmentPrefix)
 *           is present, true; otherwise, false.
 */
bool HasOption(string name, string? environmentPrefix = null)
    => HasArgument(name) || HasEnvironmentVariable(OptionNameToEnvironmentVariableName(name, environmentPrefix));

/*
 * Summary : Gets an option from, in this order:
 *             * a command line argument with the specified name;
 *             * an environment variable with the specified name converted to UNDERSCORE_UPPER_CASE;
 *             * the provided default value.
 * Params  : name         - The option name.
 *           defaultValue - The value returned if neither a corresponding argument
 *                          nor environment variable was found.
 */
T GetOption<T>(string name, T defaultValue)
    where T : notnull
    => GetOption<T>(name, null, defaultValue);

/*
 * Summary : Gets an option from, in this order:
 *             * a command line argument with the specified name;
 *             * an environment variable with the specified name converted to UNDERSCORE_UPPER_CASE
 *               and optionally prefixed with the specified environmentPrefix;
 *             * the provided default value.
 * Params  : name              - The option name.
 *           environmentPrefix - An optional prefix for the environment variable name;
 *                               for example, "camelCasedOption" with an environmentPrefix
 *                               of "MYAPP_" becomes "MYAPP_CAMEL_CASED_OPTION".
 *           defaultValue      - The value returned if neither a corresponding argument
 *                               nor environment variable was found.
 */
T GetOption<T>(string name, string? environmentPrefix, T defaultValue)
    where T : notnull
{
    var value = Context.Arguments.GetArguments(name)?.FirstOrDefault();
    if (value != null)
    {
        return ConvertOption<T>(value);
    }

    value = Context.Environment.GetEnvironmentVariable(OptionNameToEnvironmentVariableName(name, environmentPrefix));
    return value == null ? defaultValue : ConvertOption<T>(value);
}

/*
 * Summary : Gets an option from, in this order:
 *             * a command line argument with the specified name;
 *             * an environment variable with the specified name converted to UNDERSCORE_UPPER_CASE;
 *             * the provided default value.
 *           Throw an exception if the option is not found or has an empty value.
 * Params  : name - The option name.
 */
T GetOptionOrFail<T>(string name)
    where T : notnull
    => GetOptionOrFail<T>(name, null);

/*
 * Summary : Gets an option from, in this order:
 *             * a command line argument with the specified name;
 *             * an environment variable with the specified name converted to UNDERSCORE_UPPER_CASE
 *               and optionally prefixed with the specified environmentPrefix;
 *             * the provided default value.
 *           Throw an exception if the option is not found or has an empty value.
 * Params  : name              - The option name.
 *           environmentPrefix - An optional prefix for the environment variable name;
 *                               for example, "camelCasedOption" with an environmentPrefix
 *                               of "MYAPP_" becomes "MYAPP_CAMEL_CASED_OPTION".
 */
T GetOptionOrFail<T>(string name, string? environmentPrefix)
    where T : notnull
{
    var value = Context.Arguments.GetArguments(name)?.FirstOrDefault();
    if (value != null)
    {
        return ConvertOption<T>(value);
    }

    var envName = OptionNameToEnvironmentVariableName(name, environmentPrefix);
    value = Context.Environment.GetEnvironmentVariable(envName);
    if (value != null)
    {
        return ConvertOption<T>(value);
    }

    throw new CakeException($"Option {name} / environment variable {envName} not found or empty.");
}

/*
 * Summary : Converts an option name (which is supposed to be in camelCase)
 *           to an environment variable name (UNDERSCORE_UPPER_CASE).
 * Params  : prefix - An optional prefix for the environment variable name;
 *                    for example, camelCasedOption (prefix = "MYAPP_") -> MYAPP_CAMEL_CASED_OPTION
 */
// Copyright (c) 2013 Scott Kirkland - MIT License - https://github.com/srkirkland/Inflector
// Copyright (c) .NET Foundation and Contributors - MIT License - https://github.com/Humanizr/Humanizer
static string OptionNameToEnvironmentVariableName(string name, string? prefix = null)
    => (prefix ?? string.Empty) + Regex.Replace(
        Regex.Replace(
            Regex.Replace(
                name,
                @"([\p{Lu}]+)([\p{Lu}][\p{Ll}])",
                "$1_$2"),
            @"([\p{Ll}\d])([\p{Lu}])",
            "$1_$2"),
        @"[-\s]",
        "_")
        .ToUpperInvariant();

/*
 * Summary : Convert an option to the desired type.
 * Types   : T - The type to convert the option to.
 * Params  : value - The value of the option.
 * Returns : The converted value.
 */
static T ConvertOption<T>(string value)
    where T : notnull
{
    var converter = TypeDescriptor.GetConverter(typeof(T));
    return (T)converter.ConvertFromInvariantString(value)!;
}
