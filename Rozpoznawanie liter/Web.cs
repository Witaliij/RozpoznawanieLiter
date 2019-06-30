using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace Rozpoznawanie_liter
{
    //  "pojemnik" dla neuronów neuronArray
    // ładuje go po utworzeniu z pliku i zapisuje przy wyjściu

    class Web
    {
        public const int neuronInArrayWidth = 17; // ilość pozioma
        public const int neuronInArrayHeight = 17; // ilość pionowa
        private const string memory = "memory.txt"; // nazwa pliku pamięci sieciowej    memory.txt
        private List<Neuron> neuronArray = null; // tablica neuronów

        // konstruktor
        public Web()
        {
            neuronArray = InitWeb();
        }

        // funkcja otwiera plik tekstowy i konwertuje go na tablicę neuronów
        private static List<Neuron> InitWeb()
        {
            if (!File.Exists(memory)) return new List<Neuron>();
            string[] lines = File.ReadAllLines(memory);
            if (lines.Length == 0) return new List<Neuron>();
            string jStr = lines[0];
            JavaScriptSerializer json = new JavaScriptSerializer();
            List<Object> objects = json.Deserialize<List<Object>>(jStr);
            List<Neuron> res = new List<Neuron>();
            foreach (var o in objects) res.Add(NeuronCreate((Dictionary<string, Object>)o));
            return res;
        }

        // konwersja struktury danych do klasy neuronów
        private static Neuron NeuronCreate(Dictionary<string, object> o)
        {
            Neuron res = new Neuron();
            res.name = (string)o["name"];
            res.countTrainig = (int)o["countTrainig"];
            Object[] veightData = (Object[])o["veight"];
            int arrSize = (int)Math.Sqrt(veightData.Length);
            res.veight = new double[arrSize, arrSize];
            int index = 0;
            for (int n = 0; n < res.veight.GetLength(0); n++)
                for (int m = 0; m < res.veight.GetLength(1); m++)
                {
                    res.veight[n, m] = Double.Parse(veightData[index].ToString());
                    index++;
                }
            return res;
        }

        // funkcja porównuje tablicę wejściową z każdym neuronem z sieci i
        // zwraca nazwę neuronu najbardziej podobnego do niego (rozpoznawanie litery)


        public string CheckLitera(int[,] arr)
        {
            string res = null;
            double max = 0;
            foreach (var n in neuronArray)
            {
                double d = n.GetRes(arr);
                if (d > max)
                {
                    max = d;
                    res = n.GetName();
                }
            }
            return res;
        }

        // funkcja zapisuje tablicę neuronów do pliku
        public void SaveState()
        {
            JavaScriptSerializer json = new JavaScriptSerializer();
            string jStr = json.Serialize(neuronArray);
            System.IO.StreamWriter file = new System.IO.StreamWriter(memory);
            file.WriteLine(jStr);
            file.Close();
        }

        // pobiera listę nazw obrazów zapisanych w pamięci
        public string[] GetLiteras()
        {
            var res = new List<string>();
            for (int i = 0; i < neuronArray.Count; i++) res.Add(neuronArray[i].GetName());
            res.Sort();
            return res.ToArray();
        }

        // ta funkcja przechowuje neuron o nazwie trainingName
        // nowa wersja obrazu danych

        public void SetTraining(string trainingName, int[,] data)
        {
            Neuron neuron = neuronArray.Find(v => v.name.Equals(trainingName));
            if (neuron == null) // jeśli neuron o tej nazwie nie istnieje, utwórz nowy i dodaj
            {                   // do tablicy neuronów
                neuron = new Neuron();
                neuron.Clear(trainingName, neuronInArrayWidth, neuronInArrayHeight);
                neuronArray.Add(neuron);
            }
            int countTrainig = neuron.Training(data); // uczenie neuronu
            string messageStr = "Nazwa zdjecia - " + neuron.GetName() +
                                " opcje zdjecia w pamieci? - " + countTrainig.ToString();

        }
    }
}
