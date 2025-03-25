interface IHazardNotifier
{
    void Notify(string m);
}

 abstract class Container
{
    private int count = 1;
    public string SNum { get; }
    public double MLoad { get; }
    public double Sweight { get; }
    public double CurLoad { get; set; }

    protected Container(string t, double MLoad, double Sweight)
    {
        SNum = $"KON-{t}-{count++}";
        this.MLoad = MLoad;
        this.Sweight = Sweight;
    }

    public virtual void Load(double weight)
    {
        if (CurLoad + weight > MLoad)
            throw new Exception("OverfillException: Przekroczono maksymalną ładowność kontenera: " + SNum );
        CurLoad += weight;
    }

    public virtual void Unload()
    {
        CurLoad = 0;
    }

    public virtual void Info()
    {
        Console.WriteLine($"{SNum}: {MLoad} - {Sweight}");
    }
}

 class LContainer : Container, IHazardNotifier
{
    public bool IsHazard { get; }

    public LContainer(double MLoad, double Sweight, bool IsHazard) : base("L", MLoad, Sweight)
    {
        this.IsHazard = IsHazard;
        if (IsHazard)
            Notify("zawiera niebezpieczny płyn");
        {
            
        }
    }

    public override void Load(double weight)
    {
        double limit = IsHazard ? MLoad * 0.5 : MLoad * 0.9;
        if (CurLoad + weight > limit)
        {
            Notify("niebezpieczne przepełnienie kontenera");
        }
        base.Load(weight);
    }

    public void Notify(string m)
    {
        Console.WriteLine($"{SNum}: {m}");
    }

    override public void Info()
    {
        base.Info();
        Console.WriteLine($"niebezpieczne: {IsHazard}");
    }
}

class GContainer : Container, IHazardNotifier
{
    public double Pres { get; }
    public bool IsHazard { get; }
    public GContainer(double mLoad, double pres, double Sweight,bool isHazard)
        : base("G", mLoad, Sweight)
    {
        Pres= pres;
        IsHazard = isHazard;
        if (IsHazard)
            Notify("zawiera niebezpieczny gaz");
    }
    
    public override void Unload()
    {
        CurLoad *= 0.05;
    }
    
    public void Notify(string m)
    {
        Console.WriteLine($"{SNum} {m}");
    }
    override public void Info()
    {
        base.Info();
        Console.WriteLine($"niebezpieczne: {IsHazard}");
        Console.WriteLine($"cisnienie: {Pres}");
        
    }
}

class CContainer : Container, IHazardNotifier
{
    Dictionary<string, double> ProdT = new Dictionary<string, double>
    {
        { "Bananas", 13.3 },
        { "Chocolate", 18 },
        { "Fish", 2 },
        { "Meat", -15 },
        { "Ice cream", -18 },
        { "Frozen pizza", -30 },
        { "Cheese", 7.2 },
        { "Sausages", 5 },
        { "Butter", 20.5 },
        { "Eggs", 19 }
    };
    
    public double Temp{ get; }
    public string Pname { get; set; } = "";
    List<string> t = new List<string>();

    public CContainer( double MLoad, double Sweight, double Temp) : base("C", MLoad, Sweight)
    {
        this.Temp = Temp;
        prod();
        Console.WriteLine($"Kontener {SNum} moze przetransportować:");
        Console.WriteLine(string.Join("\n", t));
    }
    
    override public void Info()
    {
        base.Info();
        Console.WriteLine($"temperatura: {Temp}");
        Console.WriteLine($"Nazwa Produktu: {Pname}");
        
    }

    public void prod()
    {
        
        foreach (var product in ProdT)
        {
            if (Temp <= product.Value)
            {
                t.Add(product.Key);
            }
        }
        
    }

    public void Load(double weight,string name)
    {
        if (t.Contains(name))
        {
            if (name.Equals(Pname) || Pname.Equals(""))
            {
                Pname = name;
                base.Load(weight);
            }
            else
            {
                Console.WriteLine($"Do {SNum} mozna zaladowac tylko {Pname}");
            }
        }
        else
        {
            Console.WriteLine($"Niezgodna temperatura kontenera {SNum} z produktem{name}");
        }
    }


    public void Notify(string m)
    {
        Console.WriteLine($"{SNum} {m}");
    }

    public override void Unload()
    {
        Pname = "";
        base.Unload();
    }
    
    
}

class Ship
 {
     public List<Container> Containers { get; } = new List<Container>();
     public double MWeight { get; }
     public double Pre { get; }
     public int MContainers { get; }
    
     public Ship(double maxWeight, int maxContainers,double pre)
     {
         MWeight = maxWeight;
         MContainers = maxContainers;
         Pre = pre;
     }

     public void Info()
     {
         Console.WriteLine($"max waga: {MWeight}, containers: {MContainers}, predkosc: {Pre}");
         Console.WriteLine("Zawiera:");
         foreach (var var in Containers)
         {
             var.Info();
         }
     }

     public void LoadContainer(Container container)
     {
         double OvWeight=0;
         foreach (var con in Containers)
         {
             OvWeight += con.Sweight + con.CurLoad;
         }
         if (Containers.Count >= MContainers || OvWeight >= MWeight)
             throw new Exception("Brak miejsca na więcej kontenerów!");
         Containers.Add(container);
     }
     public void LoadContainer(List<Container> container)
     {
         double OvWeight=0;
         foreach (var v in container)
         {
             foreach (var con in Containers)
             {
                 OvWeight += con.Sweight + con.CurLoad;
             }
             if (Containers.Count >= MContainers || OvWeight >= MWeight)
                 throw new Exception("Brak miejsca na więcej kontenerów!");
             Containers.Add(v);
         }
     }

     public void Replace(Container adding, string removing)
     {
         Container todelete=null;
         foreach (var v in Containers)
         {
             if (v.SNum.Equals(removing))
                 todelete = v;
         }
            if (todelete is not null)
                Containers.Remove(todelete);
            else
             Console.WriteLine($"na statku nie ma {removing}");
         


         LoadContainer(adding);
     }

     public void Switch(Ship ship, Ship newship, Container container)
     {
         ship.Unload(container);
         newship.LoadContainer(container);
     }

     public void Unload(Container container)
     {
         Containers.Remove(container);
     }
 }


class cw3
{
    static void Main()
    {
        Ship ship = new Ship(50000, 10,50 );
        CContainer cc = new CContainer(1000, 100, 0);
        LContainer lc = new LContainer(1000, 100, true);
        GContainer gc = new GContainer(1000, 100, 100, true);

        cc.Load(200, "Bananas");
        Console.WriteLine($"{cc.CurLoad} {cc.Pname}");
        lc.Load(100);
        Console.WriteLine($"{lc.CurLoad}");
        gc.Load(100);
        gc.Unload();
        Console.WriteLine($"{gc.CurLoad}");

        ship.LoadContainer(cc);
        Console.WriteLine($"{ship.Containers.Count} - liczba kontenerow na statku");
        List<Container> containers = new List<Container>();
        containers.Add(lc);
        containers.Add(gc);
        ship.LoadContainer(containers);
        Console.WriteLine($"{ship.Containers.Count} - liczba kontenerow na statku");

        ship.Unload(cc);
        Console.WriteLine($"{ship.Containers.Count} - liczba kontenerow na statku");

        

        string n = gc.SNum;
        ship.Replace(cc,n);
        ship.Info();
        
        Ship ship2 = new Ship(50000, 10,50);
        ship.Switch(ship, ship2, cc);
        
        cc.Info();
        gc.Info();
        ship.Info();
    }
}