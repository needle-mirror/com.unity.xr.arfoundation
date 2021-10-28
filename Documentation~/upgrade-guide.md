---
uid: arfoundation-upgrade-guide
---
# Upgrading to AR Foundation version 5.0

To upgrade to AR Foundation package version 5.0, you need to do the following:

- Use Unity 2021.2 or newer.
- Be aware of the change in package structure.

**Use Unity 2021.2 or newer**

This version of the package requires Unity 2021.2 or newer.

**Be aware of the change in package structures**

The `com.unity.xr.arsubsystems` package has been merged into ARFoundation. In most cases, no action is required but if an explicit dependency on ARSubsystems Package is listed in the project then it should be replaced with `com.unity.xr.arfoundation`.

- Package Structure in `v4.2`
![Package Structure in 4.2](images/package-structure-4.2.jpg "Package Structure in 4.2")


- New package Structure in `v5.0`
![Package Structure in 5.0](images/package-structure-5.0.jpg "Package Structure in 5.0")