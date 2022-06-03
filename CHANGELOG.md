## [8.0.4](https://github.com/Elders/Cronus.Api/compare/v8.0.3...v8.0.4) (2022-06-03)


### Bug Fixes

* bump Cronus from 8.0.4 to 8.0.6 ([d83871d](https://github.com/Elders/Cronus.Api/commit/d83871d2fa7429af38c99ae4be4ae1a40f419218))

## [8.0.3](https://github.com/Elders/Cronus.Api/compare/v8.0.2...v8.0.3) (2022-06-03)


### Bug Fixes

* bump Cronus.Serialization.NewtonsoftJson from 7.0.0 to 8.0.1 ([a53c654](https://github.com/Elders/Cronus.Api/commit/a53c654848ca7e56758bbc6a5abd279dae09723a))

## [8.0.2](https://github.com/Elders/Cronus.Api/compare/v8.0.1...v8.0.2) (2022-06-03)


### Bug Fixes

* bump Cronus from 8.0.0-preview.5 to 8.0.4 ([5f9125b](https://github.com/Elders/Cronus.Api/commit/5f9125b86718a5b40385d176955c930cf5d69bed))
* bump Cronus.AspNetCore from 7.0.0 to 8.0.0 ([2f73253](https://github.com/Elders/Cronus.Api/commit/2f73253a1fc5a066dced9e4ed1056c81bb1f5c08))
* bump Cronus.Projections.Cassandra from 8.0.0-preview.3 to 8.0.1 ([b158cd8](https://github.com/Elders/Cronus.Api/commit/b158cd851e391e3dc83b4261add415ea56b4f866))
* IProjectionStore.EnumerateProjection was changed to IProjectionStore.EnumerateProjectionAsync ([f7f38f6](https://github.com/Elders/Cronus.Api/commit/f7f38f6020559dceec601e775771ba7c222c35da))

## [8.0.1](https://github.com/Elders/Cronus.Api/compare/v8.0.0...v8.0.1) (2022-05-25)


### Bug Fixes

* bump Cronus.Persistence.Cassandra from 8.0.0-preview.1 to 8.0.0 ([5270b11](https://github.com/Elders/Cronus.Api/commit/5270b1186bc17f0ef70414b65a63a46535eef1e5))
* bump Cronus.Projections.Cassandra ([6545fa9](https://github.com/Elders/Cronus.Api/commit/6545fa9633e888f9a939120681aa9289f51b8f99))
* bump Microsoft.AspNetCore.Authentication.JwtBearer ([5159e2c](https://github.com/Elders/Cronus.Api/commit/5159e2c401a9f6dc38fa8237820546c1318e2dbf))
* bump Microsoft.AspNetCore.Mvc.NewtonsoftJson from 6.0.3 to 6.0.5 ([1d46b7a](https://github.com/Elders/Cronus.Api/commit/1d46b7a46083d243814e9b20b1a2e0f23005495e))

# [8.0.0](https://github.com/Elders/Cronus.Api/compare/v7.0.1...v8.0.0) (2022-05-25)

## [7.0.1](https://github.com/Elders/Cronus.Api/compare/v7.0.0...v7.0.1) (2022-04-15)


### Bug Fixes

* Add log for event store exploring ([1d47158](https://github.com/Elders/Cronus.Api/commit/1d4715835bd43adead1175b9b16c23a0c4439682))
# [8.0.0-preview.1](https://github.com/Elders/Cronus.Api/compare/v7.0.0...v8.0.0-preview.1) (2022-04-11)

# [7.0.0](https://github.com/Elders/Cronus.Api/compare/v6.2.0...v7.0.0) (2022-04-05)


### Bug Fixes

* Add Newtonsoft.Json package ([0396b63](https://github.com/Elders/Cronus.Api/commit/0396b6398a5048eeab9cb37846295858ee3c3866))
* Configure NewtonsoftJson ([f82431b](https://github.com/Elders/Cronus.Api/commit/f82431b745e4f91acbf12803da6161716f3afbf2))
* Fix Build ([0b88828](https://github.com/Elders/Cronus.Api/commit/0b88828bbdf70b95309f7d9280ca4c74a918746b))
* Fix issue with unwraping public events ([8f9ba5a](https://github.com/Elders/Cronus.Api/commit/8f9ba5a363f8af91dc2d0c0876cbde7fd719848c))
* Made it possible to add headers for public events and their subsequent encoding. ([9d997b9](https://github.com/Elders/Cronus.Api/commit/9d997b915893c9d3f6241a42c14e1bb093981daf))
* Migrate to 6.0.3 ([4509008](https://github.com/Elders/Cronus.Api/commit/4509008279b24170d431a73dfd86a062f2f57a8e))
* Try signalR with this path ([712d76d](https://github.com/Elders/Cronus.Api/commit/712d76d9682d9c26a02ec305cf3efdd8afa11588))
* Update Cronus ([c86073b](https://github.com/Elders/Cronus.Api/commit/c86073bee024847034544ea6d5178fbc5bb22d39))
* Update Cronus ([dfa1287](https://github.com/Elders/Cronus.Api/commit/dfa12876adead0fa7cf7ad49e5eaf68bacd641a8))
* Update packages ([afc2ba7](https://github.com/Elders/Cronus.Api/commit/afc2ba7046a1df6742d503702798f3ed1e043c6a))
* Update packages ([d143471](https://github.com/Elders/Cronus.Api/commit/d143471a268ae26865b0f1fa90737fb088abfa31))
* Update packages ([27edc9f](https://github.com/Elders/Cronus.Api/commit/27edc9f04dc934a63eee326b134fbd3eedac7210))


### Features

* Adds an endpoint to force Finalize a projection build/rebuild ([2254cb5](https://github.com/Elders/Cronus.Api/commit/2254cb5a158cfd8b8a87e188a289a2b53945f600))
* Make controllers for connecting the dashboard with the monitoring service ([c298b15](https://github.com/Elders/Cronus.Api/commit/c298b15be7c5344a9cdf80bfe84743c58e7c2ee4))

# [7.0.0-preview.14](https://github.com/Elders/Cronus.Api/compare/v7.0.0-preview.13...v7.0.0-preview.14) (2022-04-04)


### Bug Fixes

* Try signalR with this path ([712d76d](https://github.com/Elders/Cronus.Api/commit/712d76d9682d9c26a02ec305cf3efdd8afa11588))

# [7.0.0-preview.13](https://github.com/Elders/Cronus.Api/compare/v7.0.0-preview.12...v7.0.0-preview.13) (2022-04-04)


### Bug Fixes

* Update packages ([d143471](https://github.com/Elders/Cronus.Api/commit/d143471a268ae26865b0f1fa90737fb088abfa31))

# [7.0.0-preview.12](https://github.com/Elders/Cronus.Api/compare/v7.0.0-preview.11...v7.0.0-preview.12) (2022-03-31)


### Bug Fixes

* Update Cronus ([c86073b](https://github.com/Elders/Cronus.Api/commit/c86073bee024847034544ea6d5178fbc5bb22d39))

# [7.0.0-preview.11](https://github.com/Elders/Cronus.Api/compare/v7.0.0-preview.10...v7.0.0-preview.11) (2022-03-31)


### Bug Fixes

* Migrate to 6.0.3 ([4509008](https://github.com/Elders/Cronus.Api/commit/4509008279b24170d431a73dfd86a062f2f57a8e))

# [7.0.0-preview.10](https://github.com/Elders/Cronus.Api/compare/v7.0.0-preview.9...v7.0.0-preview.10) (2022-03-15)


### Bug Fixes

* Fix Build ([0b88828](https://github.com/Elders/Cronus.Api/commit/0b88828bbdf70b95309f7d9280ca4c74a918746b))

# [7.0.0-preview.9](https://github.com/Elders/Cronus.Api/compare/v7.0.0-preview.8...v7.0.0-preview.9) (2022-03-14)


### Bug Fixes

* Update packages ([27edc9f](https://github.com/Elders/Cronus.Api/commit/27edc9f04dc934a63eee326b134fbd3eedac7210))

# [7.0.0-preview.8](https://github.com/Elders/Cronus.Api/compare/v7.0.0-preview.7...v7.0.0-preview.8) (2022-03-10)


### Features

* Make controllers for connecting the dashboard with the monitoring service ([c298b15](https://github.com/Elders/Cronus.Api/commit/c298b15be7c5344a9cdf80bfe84743c58e7c2ee4))

# [7.0.0-preview.7](https://github.com/Elders/Cronus.Api/compare/v7.0.0-preview.6...v7.0.0-preview.7) (2022-03-02)


### Bug Fixes

* Configure NewtonsoftJson ([f82431b](https://github.com/Elders/Cronus.Api/commit/f82431b745e4f91acbf12803da6161716f3afbf2))

# [7.0.0-preview.6](https://github.com/Elders/Cronus.Api/compare/v7.0.0-preview.5...v7.0.0-preview.6) (2022-03-02)


### Bug Fixes

* Add Newtonsoft.Json package ([0396b63](https://github.com/Elders/Cronus.Api/commit/0396b6398a5048eeab9cb37846295858ee3c3866))

# [7.0.0-preview.5](https://github.com/Elders/Cronus.Api/compare/v7.0.0-preview.4...v7.0.0-preview.5) (2022-02-16)


### Bug Fixes

* Made it possible to add headers for public events and their subsequent encoding. ([9d997b9](https://github.com/Elders/Cronus.Api/commit/9d997b915893c9d3f6241a42c14e1bb093981daf))

# [7.0.0-preview.4](https://github.com/Elders/Cronus.Api/compare/v7.0.0-preview.3...v7.0.0-preview.4) (2022-01-14)


### Features

* Adds an endpoint to force Finalize a projection build/rebuild ([2254cb5](https://github.com/Elders/Cronus.Api/commit/2254cb5a158cfd8b8a87e188a289a2b53945f600))

# [7.0.0-preview.3](https://github.com/Elders/Cronus.Api/compare/v7.0.0-preview.2...v7.0.0-preview.3) (2022-01-06)


### Bug Fixes

* Fix issue with unwraping public events ([8f9ba5a](https://github.com/Elders/Cronus.Api/commit/8f9ba5a363f8af91dc2d0c0876cbde7fd719848c))

# [7.0.0-preview.2](https://github.com/Elders/Cronus.Api/compare/v7.0.0-preview.1...v7.0.0-preview.2) (2021-11-30)


### Bug Fixes

* Update Cronus ([dfa1287](https://github.com/Elders/Cronus.Api/commit/dfa12876adead0fa7cf7ad49e5eaf68bacd641a8))

# [7.0.0-preview.1](https://github.com/Elders/Cronus.Api/compare/v6.2.0-preview.11...v7.0.0-preview.1) (2021-11-11)


### Features

* Projection rebuild progress ([4ab0906](https://github.com/Elders/Cronus.Api/commit/4ab0906bf53ac03c5862163270b5c0132aa226be))
* Projection rebuild progress ([83b2741](https://github.com/Elders/Cronus.Api/commit/83b27418aa7b47a7c949b95add2a3c76124dea5e))
* Release ([bdaf6b9](https://github.com/Elders/Cronus.Api/commit/bdaf6b924a4959d10d5c71d558bdf3996b3cdb10))

# [6.2.0-preview.11](https://github.com/Elders/Cronus.Api/compare/v6.2.0-preview.10...v6.2.0-preview.11) (2021-09-06)


### Bug Fixes

* add endpoint for replaying public event ([19a8105](https://github.com/Elders/Cronus.Api/commit/19a8105be11b175f7dc162f409224f06711ae0ce))

# [6.2.0-preview.10](https://github.com/Elders/Cronus.Api/compare/v6.2.0-preview.9...v6.2.0-preview.10) (2021-08-02)


### Bug Fixes

* Connection to SignalR hubs does not require tenant resolve ([8e27716](https://github.com/Elders/Cronus.Api/commit/8e27716d2a8d911c62110e5d62e177718dd4c006))

# [6.2.0-preview.9](https://github.com/Elders/Cronus.Api/compare/v6.2.0-preview.8...v6.2.0-preview.9) (2021-08-02)


### Bug Fixes

* Code cleanup ([96b94ab](https://github.com/Elders/Cronus.Api/commit/96b94aba4e4bee86bfefd57d6e142edabb82f757))

# [6.2.0-preview.8](https://github.com/Elders/Cronus.Api/compare/v6.2.0-preview.7...v6.2.0-preview.8) (2021-07-06)


### Bug Fixes

* Add additional message headers to the republish api ([1becc07](https://github.com/Elders/Cronus.Api/commit/1becc0758749355be29d2427c961ea8d4e825c5d))

# [6.2.0-preview.7](https://github.com/Elders/Cronus.Api/compare/v6.2.0-preview.6...v6.2.0-preview.7) (2021-05-19)


### Bug Fixes

* Adds ping controller ([f46bc16](https://github.com/Elders/Cronus.Api/commit/f46bc1619d5cc27b3099c72aebb207678e190c67))

# [6.2.0-preview.6](https://github.com/Elders/Cronus.Api/compare/v6.2.0-preview.5...v6.2.0-preview.6) (2021-05-13)


### Bug Fixes

* Fixes how handlers are collected. We do not load abstract handler anymore ([24480cd](https://github.com/Elders/Cronus.Api/commit/24480cdb04478cad8da6b0b0274c26e1dd947891))

# [6.2.0-preview.5](https://github.com/Elders/Cronus.Api/compare/v6.2.0-preview.4...v6.2.0-preview.5) (2021-05-12)


### Bug Fixes

* Updates Cronus ([adddde3](https://github.com/Elders/Cronus.Api/commit/adddde3e5e44f3f7d2c8adfef2cf8151910d2fda))

# [6.2.0-preview.4](https://github.com/Elders/Cronus.Api/compare/v6.2.0-preview.3...v6.2.0-preview.4) (2021-05-11)


### Bug Fixes

* Updates copyright attribute ([d30b3d9](https://github.com/Elders/Cronus.Api/commit/d30b3d92595ecf0bb4d3614b63978be3aebfadb6))
* Updates packages ([cd7341b](https://github.com/Elders/Cronus.Api/commit/cd7341b3452462bc0d2850cd9a055039e073acbc))

# [6.2.0-preview.3](https://github.com/Elders/Cronus.Api/compare/v6.2.0-preview.2...v6.2.0-preview.3) (2021-05-07)


### Bug Fixes

* Adds ID to the Ports response ([c8e791c](https://github.com/Elders/Cronus.Api/commit/c8e791c5828031e71380196f5ccf118d2829266a))

# [6.2.0-preview.2](https://github.com/Elders/Cronus.Api/compare/v6.2.0-preview.1...v6.2.0-preview.2) (2021-05-07)


### Bug Fixes

* Updates packages ([24941d2](https://github.com/Elders/Cronus.Api/commit/24941d29c00c92630617ea3ea3b345cad1dd39ea))

# [6.2.0-preview.1](https://github.com/Elders/Cronus.Api/compare/v6.1.4...v6.2.0-preview.1) (2021-05-07)


### Bug Fixes

* Removes gitversion ([62aaa18](https://github.com/Elders/Cronus.Api/commit/62aaa18d9a7c029ec599c2f4910f9a197b39d0e8))


### Features

* Projection rebuild progress ([4ab0906](https://github.com/Elders/Cronus.Api/commit/4ab0906bf53ac03c5862163270b5c0132aa226be))
* Projection rebuild progress ([83b2741](https://github.com/Elders/Cronus.Api/commit/83b27418aa7b47a7c949b95add2a3c76124dea5e))

# 1.0.0-preview.1 (2021-05-07)


### Bug Fixes

* Removes gitversion ([62aaa18](https://github.com/Elders/Cronus.Api/commit/62aaa18d9a7c029ec599c2f4910f9a197b39d0e8))


### Features

* Projection rebuild progress ([4ab0906](https://github.com/Elders/Cronus.Api/commit/4ab0906bf53ac03c5862163270b5c0132aa226be))
* Projection rebuild progress ([83b2741](https://github.com/Elders/Cronus.Api/commit/83b27418aa7b47a7c949b95add2a3c76124dea5e))

# 1.0.0-preview.1 (2021-05-07)


### Bug Fixes

* Removes gitversion ([62aaa18](https://github.com/Elders/Cronus.Api/commit/62aaa18d9a7c029ec599c2f4910f9a197b39d0e8))


### Features

* Projection rebuild progress ([4ab0906](https://github.com/Elders/Cronus.Api/commit/4ab0906bf53ac03c5862163270b5c0132aa226be))
* Projection rebuild progress ([83b2741](https://github.com/Elders/Cronus.Api/commit/83b27418aa7b47a7c949b95add2a3c76124dea5e))
