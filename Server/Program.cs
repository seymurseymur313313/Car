
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Collections.ObjectModel;
using System.Net.Http;

namespace Server
{
    internal class Program
    {
        // Methodlarda gorsun deye static edilib!!!
        public static ObservableCollection<Car> Car = new ObservableCollection<Car>();
        static void Main()
        {
            UdpClient udpClient = new UdpClient(27001);
            var remoteEp = new IPEndPoint(IPAddress.Any, 0); 
            Car = ReadData<ObservableCollection<Car>>("Cars")!;
            Console.WriteLine("Server Started Succsessfuly...");
            while (true)
            {
                var buffer = new byte[1024];
                dynamic chunks;
                var result = udpClient.Receive(ref remoteEp);
                var request = Encoding.Default.GetString(result);
                var command = System.Text.Json.JsonSerializer.Deserialize<Command>(request);
                // Methods
                switch (command!.Method)
                {
                    // Get
                    case HttpMethods.GET:
                        request = System.Text.Json.JsonSerializer.Serialize(Car);
                        buffer = Encoding.Default.GetBytes(request);
                        chunks = buffer.Chunk(ushort.MaxValue - 29);

                        foreach (var chunk in chunks)
                        {
                            udpClient.Send(chunk, chunk.Length, remoteEp);
                        }
                        break;
                    // Post
                    case HttpMethods.POST:
                        Add(command.Car);
                        request = System.Text.Json.JsonSerializer.Serialize(Car);
                        buffer = Encoding.Default.GetBytes(request);
                        chunks = buffer.Chunk(ushort.MaxValue - 29);

                        foreach (var chunk in chunks)
                        {
                            udpClient.Send(chunk, chunk.Length, remoteEp);
                        }
                        break;
                    // Put
                    case HttpMethods.PUT:
                        Update(command.Car);
                        request = System.Text.Json.JsonSerializer.Serialize(Car);
                        buffer = Encoding.Default.GetBytes(request);
                        chunks = buffer.Chunk(ushort.MaxValue - 29);

                        foreach (var chunk in chunks)
                        {
                            udpClient.Send(chunk, chunk.Length, remoteEp);
                        }
                        break;
                    //  Delete By Id
                    case HttpMethods.DELETE:
                        Delete(command.Id);
                        request = System.Text.Json.JsonSerializer.Serialize(Car);
                        buffer = Encoding.Default.GetBytes(request);
                        chunks = buffer.Chunk(ushort.MaxValue - 29);
                        foreach (var chunk in chunks)
                        {
                            udpClient.Send(chunk, chunk.Length, remoteEp);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        // Get Methods
        Car? GetById(int id)
        {

            foreach (Car car in Car)
            {
                if (car.Id == id) return car;
            }
            return null;
        }
        // Return All Vehicles
        static ObservableCollection<Car>? GetAll()
        {
            return Car;
        }
        // Add Vehicle
        static bool Add(Car car)
        {

            foreach (var item in Car)
            {
                if (item.Id == car.Id) return false;
            }
            Car.Add(car);
            WriteData(Car, "Cars");
            return true;
        }
        // Update Method
        static bool Update(Car car)
        {

            foreach (var item in Car)
            {
                if (item.Id == car.Id)
                {
                    item.CloneFromAnotherInstance(car);
                    return true;
                }
            }
            WriteData(Car, "Cars");
            return false;
        }
        // Delete Method
        static bool Delete(int id)
        {
            foreach (var item in Car)
            {
                if (item.Id == id)
                {
                    Car.Remove(item);
                    return true;
                }
            }
            WriteData(Car, "Cars");
            return false;
        }
    }
}
