# Changelog

All notable changes to AaaLib will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## Unreleased changes

### New features

### Changes to existing features

### Bugs fixed in this release

### Known problems introduced by this release

## [2.0.67](https://github.com/Tenacom/AaaLib/releases/tag/2.0.67) (2022-11-30)

The [`NodaTime`](https://github.com/nodatime/nodatime) dependency has been updated to keep up with changes in the time zone database.

## [2.0.58](https://github.com/Tenacom/AaaLib/releases/tag/2.0.58) (2022-11-25)

### Changes to existing features

- AaaLib now references a stable version of `PolyKit`.

### Bugs fixed in this release

- A bug in the GitHub Pages deployment workflow has been fixed: online documentation is again available.

## [2.0.47](https://github.com/Tenacom/AaaLib/releases/tag/2.0.47) (2022-11-23)

### New features

- .NET 7 was added as a target platform.

### Changes to existing features

- **BREAKING CHANGE:** Following .NET's [Library support for older frameworks](https://learn.microsoft.com/en-us/dotnet/core/compatibility/core-libraries/7.0/old-framework-support) policy, support for .NET Core 3.1 has been removed.
- **BREAKING CHANGE:** Library code has been completely rewritten and is now a less feeble excuse for real library code.
- Unit tests for all library code have been added.
- Several additions and improvements have been made to CI scripts. Most notably:
  - Code coverage data is automatically collected and uploaded to Codecov.
  - Version increments are now automatic when creating a new release, according to changes in public API files. Increments can also be forced by setting the "Version spec change" workflow parameter to "Major" or "Minor".

### Bugs fixed in this release

- When creating a new release, it was previously possible to increment the version specification more than once, e.g. from 1.0 to 3.0. This has been fixed: superflous version increments with respect to latest stable version are now ignored.
- The CodeQL workflow only employed a subset of the available checks. Quality checks are now performed and any resulting alert appears in the Security tab of the repository.

## [1.0.60](https://github.com/Tenacom/AaaLib/releases/tag/1.0.60) (2022-10-14)

No changes. _Still_ testing the release workflow.

## [1.0.52](https://github.com/Tenacom/AaaLib/releases/tag/1.0.52) (2022-10-14)

No changes. Just testing the release workflow.

## [1.0.47](https://github.com/Tenacom/AaaLib/releases/tag/1.0.47) (2022-10-13)

Initial release.
