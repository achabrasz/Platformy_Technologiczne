package main.java.org.example;
import java.util.*;
//import Mage;

public class Main {
    public static void main(String[] args) {
        Set<Mage> mages;
        int mode = 0;
        if (args[0].equals("none") || args[0].isEmpty()) {
            mages = new HashSet<Mage>();
        } else {
            mages = new TreeSet<Mage>();
            mode = 1;
        }

        Mage mage1 = new Mage("Ryze", 10, 100, mode);
        Mage mage2 = new Mage("Syndra", 8, 80, mode);
        Mage mage3 = new Mage("Annie", 3, 20, mode);
        Mage mage4 = new Mage("Aurelion Sol", 7, 75, mode);
        Mage mage5 = new Mage("Orianna", 5, 60, mode);
        Mage mage6 = new Mage("Sylas", 9, 93, mode);
        Mage mage7 = new Mage("Renata", 4, 47, mode);
        Mage mage8 = new Mage("Cassiopeia", 6, 63, mode);
        Mage mage9 = new Mage("Vladimir", 7, 78, mode);
        Mage mage10 = new Mage("Anivia", 2, 23, mode);

        mage1.AddApprentice(mage6);
        mage6.AddApprentice(mage5);
        mage9.AddApprentice(mage8);

        mages.add(mage1);
        mages.add(mage2);
        mages.add(mage3);
        mages.add(mage4);
        mages.add(mage5);
        mages.add(mage6);
        mages.add(mage7);
        mages.add(mage8);
        mages.add(mage9);
        mages.add(mage10);

        //System.out.println(mage1);
        System.out.println(mages);
        generateStatistics(mages, mode);
        //mage1.PrintAll();
    }

    public static void generateStatistics(Set<Mage> mages, int mode) {
        Map<Mage, Integer> statistics;

        // Sprawdź parametr sortowania
        if (mode == 0) {
            statistics = new HashMap<>();
        } else {
            statistics = new TreeMap<>();
        }

        // Generowanie statystyk
        for (Mage mage : mages) {
            int descendants = countDescendants(mage);
            statistics.put(mage, descendants);
        }

        // Wypisanie statystyk
        System.out.println("Statistics:");
        for (Map.Entry<Mage, Integer> entry : statistics.entrySet()) {
            System.out.println(entry.getKey().toString() + " - " + entry.getValue() + " descendants");
        }
    }

    // Metoda rekurencyjnie licząca liczbę potomków
    public static int countDescendants(Mage mage) {
        int count = mage.getApprentices().size();
        for (Mage apprentice : mage.getApprentices()) {
            count += countDescendants(apprentice);
        }
        return count;
    }
}