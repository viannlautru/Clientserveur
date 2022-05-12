namespace CommunicationBetween;

public  class Personne
{
    public int id { get; set; }
    public int age { get; set; }
    public string name { get; set; }

    public Personne(int age, string name)
    {
        this.age = age; this.name = name;
    }
}
