#### 6.2.0-beta0006 - 31.03.2021
* Updates Cronus

#### 6.2.0-beta0005 - 23.03.2021
* Fixes the projection commit timestamp

#### 6.2.0-beta0004 - 22.03.2021
* Separates the projection events from the state
  
#### 6.2.0-beta0003 - 19.03.2021
* Adds projection's events to Projection Explore API

#### 6.2.0-beta0002 - 16.03.2021
* Configures CORS

#### 6.2.0-beta0001 - 16.03.2021
* Projection rebuild progress

#### 6.1.4 - 22.12.2020
* Fixes how the IDs are parsed

#### 6.1.3 - 22.12.2020
* Allows to republish events

#### 6.1.2 - 26.11.2020
* Adds additional info when querying and aggregate from the event store

#### 6.1.1 - 18.09.2020
* Allows to finalize an index rebuild request manually

#### 6.1.0 - 24.08.2020
* Fixes how the URN ids are parsed
* Fixes strange problem with serialization
* Fixes how we inject the current BoundedContext

#### 6.0.0 - 16.04.2020
* Removes accidentally added reference to the playground project
* Added Playground
* Added Domain Explore API

#### 5.3.0 - 28.12.2018
* Updates Cronus

#### 5.2.1 - 10.12.2018
* Updates Cronus

#### 5.2.0 - 10.12.2018
* Adds the ability to host the API under HTTPS

#### 5.1.0 - 08.12.2018
* Adds an extension point to override Cronus services
* Updates to DNC 2.2

#### 5.0.1 - 03.12.2018
* ReRelease

#### 5.0.0 - 22.11.2018
* Returns projection with status not_present if we are unable to load it from the DB
* Configures CORS as: AllowAnyOrigin, AllowAnyMethod, AllowAnyHeader, AllowCredentials
* Fixes API hosting for windows
* Improves startup configuration
* Adds the option to run the API as a service
* Adds CronusControllerFactory 
* Adds Authentication 
* Only supports for dotnetcore
* Returning projection version revision as a number
* Force projection rebuilding
* Projection meta data api
* Projections' list now exposes the entire history of all projection versions

#### 0.1.2 - 01.03.2018
* Improves the log info start message

#### 0.1.1 - 28.02.2018
* Configures camelCase json formatter response

#### 0.1.0 - 28.02.2018
* Initial Release
