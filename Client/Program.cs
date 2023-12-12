using System.Net;
using System.Text;
using Models.Enums;
using Models.Classes;
using System.Text.Json;
using System.Net.Sockets;
using System.Net.Http;

namespace Client
{
    internal class Program
    {
        static void Main()
        {
            UdpClient udpClient = new UdpClient();
            var connectEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 27001);

            while (true)
            {

                dynamic request, jsonformat, buffer;
                string marka, model, vin, color, id, year;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("1)Butun Masinlar");
                Console.WriteLine("2)Yeni Masin");
                Console.WriteLine("3)Masin Melumatlarinin Yenilenmesi");
                Console.WriteLine("4)Movcud Masinin Silinmesi");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Secim Nomresini Daxil Edin->> ");
                var secim = Console.ReadLine();
                Console.Clear();
                Console.ResetColor();




                switch (secim)
                {
                    case "1":
                        request = new Command() { Method = HttpMethods.GET };
                        jsonformat = JsonSerializer.Serialize(request);
                        buffer = Encoding.Default.GetBytes(jsonformat);
                        udpClient.Send(buffer, buffer.Length, connectEP);

                        break;
                    case "2":
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Yeni Masin Melumatlarini Daxil Edin");
                        Console.ResetColor();
                        Console.Write("Id->> ");
                        id = Console.ReadLine();
                        Console.Write("\nMarka->> ");
                        marka = Console.ReadLine();
                        Console.Write("\nModel->> ");
                        model = Console.ReadLine();
                        Console.Write("\nVin->> ");
                        vin = Console.ReadLine();
                        Console.Write("\nColor->> ");
                        color = Console.ReadLine();
                        Console.Write("\nYear->> ");
                        year = Console.ReadLine();

                        request = new Command() { Method = HttpMethods.POST, Car = new Car(int.Parse(id), int.Parse(year), model, marka, vin, color) };
                        jsonformat = JsonSerializer.Serialize(request);
                        buffer = Encoding.Default.GetBytes(jsonformat);
                        udpClient.Send(buffer, buffer.Length, connectEP);
                        break;
                    case "3":
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Deyistirmek Isdediyiniz Melumatlari Daxil Edin");
                        Console.ResetColor();
                        Console.Write("Id->> ");
                        id = Console.ReadLine();
                        Console.Write("\nMarka->> ");
                        marka = Console.ReadLine();
                        Console.Write("\nModel->> ");
                        model = Console.ReadLine();
                        Console.Write("\nVin->> ");
                        vin = Console.ReadLine();
                        Console.Write("\nColor->> ");
                        color = Console.ReadLine();
                        Console.Write("\nYear->> ");
                        year = Console.ReadLine();

                        request = new Command() { Method = HttpMethods.PUT, Car = new Car(int.Parse(id), int.Parse(year), model, marka, vin, color) };
                        jsonformat = JsonSerializer.Serialize(request);
                        buffer = Encoding.Default.GetBytes(jsonformat);
                        udpClient.Send(buffer, buffer.Length, connectEP);
                        break;
                    case "4":
                        Console.Write("Silmek Istediyiniz Masinin Id Sini Daxil Edin->> ");
                        id = Console.ReadLine();
                        request = new Command() { Method = HttpMethods.DELETE, Id = int.Parse(id) };
                        jsonformat = JsonSerializer.Serialize(request);
                        buffer = Encoding.Default.GetBytes(jsonformat);
                        udpClient.Send(buffer, buffer.Length, connectEP);
                        break;
                    default:
                        continue;
                }






                var bytes = new List<byte>();
                while (true)
                {
                    var result = udpClient.Receive(ref connectEP);
                    var len = result.Length;
                    bytes.AddRange(result);
                    if (len != ushort.MaxValue - 29) break;
                }
                var cars = Encoding.Default.GetString(bytes.ToArray());
                List<Car>? Cars = JsonSerializer.Deserialize<List<Car>>(cars);

                Console.Clear();
                foreach (var car in Cars)
                    Console.WriteLine(car);
            }
        }
    }
}
