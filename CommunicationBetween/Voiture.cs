namespace CommunicationBetween;

public class Voiture
{
    private int carburant;
    public string marque { get; set; }
    public string modele { get; set; }
    public static int capacite = 300;

    public Voiture(string marque, string modele)
    {
        this.marque = marque;
        this.modele = modele;
        carburant = 0;
    }

    public void SetCarburant(int c) {
        int max = capacite - carburant;
        if (c < max)
        {
            carburant += c;
            Console.WriteLine("La voiture a été ravitailée");
        }
        else
        {
            carburant = capacite;
            Console.WriteLine(c - max + " litre(s) ont débordé.");
        }
    }

    public int GetCarburant()
    {
        return carburant;
    }

    public static int GetCapacite()
    {
        return capacite;
    }
}
