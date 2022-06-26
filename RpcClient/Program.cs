using RpcClient;
using static System.Console;

var client = new Client();
var input = "5";
do
{
    WriteLine($"[x] Requesting Factorial({input})");
    var response = client.Call(input);
    
    WriteLine("Enter another number to continue or any alphabet to exit.");
    input = ReadLine();
} while (!string.IsNullOrEmpty(input) &&
         int.TryParse(input, out var num));

client.Close();
ReadKey();