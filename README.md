Mimimal repo, with code generation checked in.

Application creates database on boot.

Contains two endpoints, when running in multi-tenancy mode with durable local queues, the Storage.Insert is not persisted if the endpoint returns cascading messages.
