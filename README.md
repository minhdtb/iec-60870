# [IEC-60870](https://github.com/minhdtb/IEC-60870/)

IEC-60870 is C# version of OpenMUC IEC-60870 library

## Installation
A [nuget package](https://www.nuget.org/packages/IEC60870/) is available for the library. To install `IEC60870 Library`, run the following command in the Package Manager Console:

    PM> Install-Package IEC60870

## Examples

### Client
Write your simple client application (master) like this
```csharp
 var client = new ClientSAP("127.0.0.1", 2404);
 client.NewASdu += asdu => {
      // process received Asdu
  };

  client.ConnectionClosed += e =>
  {
      Console.WriteLine(e);
  };

  client.Connect();
```

### Server
and if you want to create server application (slave), you must use ServerSAP instead of ClientSAP
```csharp
  var server = new ServerSAP("127.0.0.1", 2405); 
  server.StartListen(10);
  server.SendASdu(asdu);         
```

## License

IEC-60870 is licensed under [MIT](http://www.opensource.org/licenses/mit-license.php "Read more about the MIT license form"). Refer to license.txt for more information.
