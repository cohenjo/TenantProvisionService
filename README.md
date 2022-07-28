# Tenant Provision Service

The Azure-Schema-Operator manages schemas in a declerative a-Sync manner.
Sometimes we prefer an online sync approach for immeidate result (e.g. when provisioning a new tenant)
We can achive this by creating a `ClusterExecuter` object with the tenant info and "watching" for the `Executed` condition.

This C# playground project uses the C# kubernetes-client to create a ClusterExecuter CR.
It uses the generic client.

This is a Playground - not meant or destioned for production.


Sample `Post` call:

```json
{
  "id": "tester",
  "cluster": "schematest.database.windows.net",
  "db": "db2",
  "schema": "tester1",
  "sourceName": "dacpac-test-schema",
  "sourceNamespace": "default"
}
```


Kusto tenant (new DB)

```json
{
  "id": "kusto-tester",
  "cluster": "https://kustoschema.westeurope.kusto.windows.net",
  "db": "db1000",
  "sourceName": "dev-template-kql",
  "sourceNamespace": "default",
  "type": "kusto"
}
```