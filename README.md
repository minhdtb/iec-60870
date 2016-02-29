# [IEC-60870](https://github.com/minhdtb/IEC-60870/)

IEC-60870 is C# version of OpenMUC IEC-60870 library

## Examples

### Client

```c#
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

