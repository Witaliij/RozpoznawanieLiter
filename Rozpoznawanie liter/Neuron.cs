using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rozpoznawanie_liter
{
    // klasa neuronów, każdy neuron przechowuje tablicę obrazu, może nauczyć się 
    // i porównać wartość z tą w pamięci
    public class Neuron
    {

        public string name; // nazwa - wartość tekstowa obrazu, który przechowuje neuron
        public double[,] veight; // tablica skal - pamięć neuronu
        public int countTrainig; // liczba wariantów obrazu w pamięci, ilość przekazanych wzorców
                                 // służy do prawidłowej konwersji wag podczas treningu

        // konstruktor
        public Neuron() { }

        // pobieranie nazwy neuronu
        public string GetName() { return name; }

        // oczyszczanie pamięci neuronu i przypisanie mu nowej nazwy
        public void Clear(string name, int x, int y)
        {
            this.name = name;
            veight = new double[x, y];
            for (int n = 0; n < veight.GetLength(0); n++)
                for (int m = 0; m < veight.GetLength(1); m++) veight[n, m] = 0;
            countTrainig = 0;
        }

        // funkcja zwraca sumę odchylenia tablicy wejściowej od odniesienia
        // im wynik jest bliższy 1, tym bardziej podobna jest tablica wejściowa
        // na obrazie z pamięci neuronu
        public double GetRes(int[,] data)
        {
            if (veight.GetLength(0) != data.GetLength(0) || veight.GetLength(1) != data.GetLength(1)) return -1;
            double res = 0;
            for (int n = 0; n < veight.GetLength(0); n++)
                for (int m = 0; m < veight.GetLength(1); m++)
                    res += 1 - Math.Abs(veight[n, m] - data[n, m]);  
                                                                     // każdy element tablicy wejściowej z
                                                                     // średnią wartością z pamięci

            return res / (veight.GetLength(0) * veight.GetLength(1));// zwraca średnią arytmetyczną tablicy

        }

        // dodaj obraz wejściowy do pamięci tablicy        
        public int Training(int[,] data)
        {
            // sprawdź, czy tablica istnieje i ma taki sam rozmiar jak tablica pamięci             
            if (data == null || veight.GetLength(0) != data.GetLength(0) || veight.GetLength(1) != data.GetLength(1)) return countTrainig;
            countTrainig++;
            for (int n = 0; n < veight.GetLength(0); n++)
                for (int m = 0; m < veight.GetLength(1); m++)
                {
                 
                    double v = data[n, m] == 0 ? 0 : 1;

                    // każdy element w pamięci jest przeliczany na podstawie wartości danych
                    veight[n, m] += 2 * (v - 0.5f) / countTrainig;
                    if (veight[n, m] > 1) veight[n, m] = 1; // wartość pamięci nie może być większa niż 1
                    if (veight[n, m] < 0) veight[n, m] = 0; // wartość pamięci nie może być mniejsza niż 0
                }
            return countTrainig; //zwróć liczbę szkoleń
        }
    }
}
