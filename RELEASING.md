# Releasing

1. Bump the version in `Ternwire.csproj` and merge it to `main`.
2. Tag that commit and push the tag:

   ```sh
   git tag v0.1.1 && git push origin v0.1.1
   ```

3. The Publish workflow checks that the tag matches the version, runs the checks, and
   waits for a maintainer to approve the `release` environment. Approve it in the
   workflow run.

Published versions are immutable, and the repository blocks moving or deleting `v*`
tags. To fix a bad release, publish a new version.

## One-time setup

Publishing uses NuGet trusted publishing, so no API key is stored. On nuget.org, open
Trusted Publishing under your account and add a GitHub policy with owner `ternwire`,
repository `ternwire-dotnet`, workflow `publish.yml`, and environment `release`. Then
set the repository variable `NUGET_USER` to your nuget.org profile name.
