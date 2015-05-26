using System.IO;
using Domain;

namespace GPRMCconverter
{
    class Program
    {
        static void Main(string[] args)
        {
            var worker = new TrackXmlWorker();
            var parser = new GPRMCparser();
            const string filename = @"C:\Users\Rostislav\Google Диск\bach\gps_tracks\track 3\Output_COM19.txt";
            using (var reader = File.OpenText(filename))
            {
                var result = parser.Parse(reader);
                worker.WriteTrack(new Track{Coordinates = result}, "gps19.xml");
            }
        }
    }
}
