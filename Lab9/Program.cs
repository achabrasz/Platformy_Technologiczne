using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

class Program
{
    static void Main(string[] args)
    {
        List<Car> myCars = new List<Car>()
        {
            new Car("E250", new Engine(1.8, 204, "CGI"), 2009),
            new Car("E350", new Engine(3.5, 292, "CGI"), 2009),
            new Car("A6", new Engine(2.5, 187, "FSI"), 2012),
            new Car("A6", new Engine(2.8, 220, "FSI"), 2012),
            new Car("A6", new Engine(3.0, 295, "TFSI"), 2012),
            new Car("A6", new Engine(2.0, 175, "TDI"), 2011),
            new Car("A6", new Engine(3.0, 309, "TDI"), 2011),
            new Car("S6", new Engine(4.0, 414, "TFSI"), 2012),
            new Car("S8", new Engine(4.0, 513, "TFSI"), 2012)
        };

        var query1 = myCars
            .Where(car => car.Model == "A6")
            .Select(car => new
            {
                engineType = car.Motor.FuelType == "TDI" ? "diesel" : "petrol",
                hppl = (double)car.Motor.HorsePower / car.Motor.Displacement
            });

        var query2 = query1
            .GroupBy(item => item.engineType)
            .Select(group => new
            {
                EngineType = group.Key,
                AvgHPPL = group.Average(item => item.hppl)
            });

        foreach (var item in query1)
        {
            Console.WriteLine($"engineType: {item.engineType}, hppl: {item.hppl}");
        }

        foreach (var item in query2)
        {
            Console.WriteLine($"{item.EngineType}: {item.AvgHPPL}");
        }

        SerializeToXml(myCars, "CarsCollection.xml");
        DeserializeFromXml("CarsCollection.xml");

        XPathStatements("CarsCollection.xml");

        CreateXmlFromLinq(myCars);
        DeserializeFromXml("CarsFromLinq.xml");

        GenerateXHTMLTable(myCars);
        //string xhtmlContent = File.ReadAllText("CarsTable.xhtml");
        //Console.WriteLine(xhtmlContent);

        ModifyXmlDocument("CarsCollection.xml");
        DeserializeFromXml("CarsCollection.xml");
    }

    static void SerializeToXml(List<Car> cars, string filePath)
    {
        var xmlCars = new XElement("cars",
            cars.Select(car =>
                new XElement("car",
                    new XAttribute("model", car.Model),
                    new XElement("engine",
                        new XAttribute("model", car.Motor.Model),
                        new XElement("displacement", car.Motor.Displacement),
                        new XElement("horsePower", car.Motor.HorsePower),
                        new XElement("fuelType", car.Motor.FuelType)
                    ),
                    new XElement("year", car.Year)
                )
            )
        );

        xmlCars.Save(filePath);
    }

    static void DeserializeFromXml(string filePath)
    {
        XElement root = XElement.Load(filePath);
        Console.WriteLine(root);
    }

    private static void XPathStatements(string filePath)
    {
        XElement rootNode = XElement.Load(filePath);
        XPathNavigator navigator = rootNode.CreateNavigator();

        var countAvarageXPath = "sum(//car/engine[@model!=\"TDI\"]/horsePower) div count(//car/engine[@model!=\"TDI\"]/horsePower)";
        Console.WriteLine($"Średnia: {(double)rootNode.XPathEvaluate(countAvarageXPath)}");

        var removeDuplicatesXPath = "//car[not(@model = preceding::car[@model])]";

        XPathNodeIterator iterator = navigator.Select(removeDuplicatesXPath);

        Console.WriteLine("\nModele samochodów bez powtorzen:");
        while (iterator.MoveNext())
        {
            string model = iterator.Current.GetAttribute("model", "");
            Console.WriteLine(model);
        }
    }

    static void CreateXmlFromLinq(List<Car> myCars)
    {
        IEnumerable<XElement> nodes = from car in myCars
                                      select new XElement("car",
                                                 new XAttribute("model", car.Model),
                                                 new XElement("engine",
                                                     new XAttribute("model", car.Motor.Model),
                                                     new XElement("displacement", car.Motor.Displacement),
                                                     new XElement("horsePower", car.Motor.HorsePower),
                                                     new XElement("fuelType", car.Motor.FuelType)
                                                 ),
                                                 new XElement("year", car.Year)
                                             );

        XElement rootNode = new XElement("cars", nodes);
        rootNode.Save("CarsFromLinq.xml");
    }


    static void GenerateXHTMLTable(List<Car> myCars)
    {
        var table = new XElement("table",
            new XElement("tr",
                new XElement("th", "Model"),
                new XElement("th", "Engine"),
                new XElement("th", "Year")
            ),
            myCars.Select(car =>
                new XElement("tr",
                    new XElement("td", car.Model),
                    new XElement("td", car.Motor.Model),
                    new XElement("td", car.Year)
                )
            )
        );

        var xhtmlDocument = new XDocument(
            new XElement("html",
                new XElement("body", table)
            )
        );

        xhtmlDocument.Save("CarsTable.xhtml");
    }

    static void ModifyXmlDocument(string filePath)
    {
        XElement root = XElement.Load(filePath);
        foreach (var carElement in root.Elements("car"))
        {
            carElement.Element("engine").Element("horsePower").Name = "hp";
            var yearElement = carElement.Element("year");
            if (yearElement != null)
            {
                carElement.Add(new XAttribute("year", yearElement.Value));
                yearElement.Remove();
            }
        }
        root.Save(filePath);
    }
}

class Car
{
    public string Model { get; }
    public Engine Motor { get; }
    public int Year { get; }

    public Car(string model, Engine motor, int year)
    {
        Model = model;
        Motor = motor;
        Year = year;
    }
}

class Engine
{
    public double Displacement { get; }
    public int HorsePower { get; }
    public string FuelType { get; }
    public string Model { get; }

    public Engine(double displacement, int horsePower, string fuelType)
    {
        Displacement = displacement;
        HorsePower = horsePower;
        FuelType = fuelType;
        Model = $"{fuelType} {horsePower}HP";
    }
}

