namespace CommunicationBetween;

public  class Personne
{
    public int id { get; set; }
    public int age { get; set; }
    public string name { get; set; }
    public string ip { get; set; }
    Random aleatoire = new Random();
    
    public Personne(int age, string name,string ip)
    {
        int idalea = aleatoire.Next(1, 1300000);
        this.id = idalea;
        this.age = age; this.name = name;this.ip = ip;
    }
}
