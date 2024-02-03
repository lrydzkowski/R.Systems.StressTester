# R.Systems.StressTester

A simple console app for stress tests. It's written in .NET 8 with [Cocona](https://github.com/mayuki/Cocona) library.

## How to run it

```powershell
R.Systems.StressTester.Cli.exe --file-path <path_to_configuration_file>
```

A configuration file can be created based on the example file: `R.Systems.StressTester.Cli/Docs/send_requests_example.json`.

```json
{
  "url": "https://api.com",
  "httpMethod": "GET",
  "body": null,
  "headers": [
    {
      "name": "Authorization",
      "value": "Bearer <token>"
    }
  ]
}
```
