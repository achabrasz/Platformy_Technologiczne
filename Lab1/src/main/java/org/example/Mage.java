package main.java.org.example;
import java.util.*;

public class Mage implements Comparable{
    private String name;
    private int level;
    private double power;
    private int mode;
    private Set<Mage> apprentices = new HashSet<Mage>();

    private MageComparator comparator = new MageComparator();

    public Mage(String name, int level, double power, int mode) {
        this.name = name;
        this.level = level;
        this.power = power;
        this.mode = mode;
    }

    public void AddApprentice(Mage mage) {
        apprentices.add(mage);
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public int getLevel() {
        return level;
    }

    public void setLevel(int level) {
        this.level = level;
    }

    public double getPower() {
        return power;
    }

    public void setPower(double power) {
        this.power = power;
    }

    public Set<Mage> getApprentices() {
        return apprentices;
    }

    public boolean equals(Object object) {
        if (this == object) return true;
        if (object == null || getClass() != object.getClass()) return false;
        if (!super.equals(object)) return false;
        Mage mage = (Mage) object;
        return getLevel() == mage.getLevel() && java.lang.Double.compare(mage.getPower(), getPower()) == 0 && java.util.Objects.equals(getName(), mage.getName()) && java.util.Objects.equals(apprentices, mage.apprentices);
    }

    public int hashCode() {
        return Objects.hash(super.hashCode(), getName(), getLevel(), getPower());
    }

    @java.lang.Override
    public java.lang.String toString() {
        return "Mage{" + "name='" + name + '\'' + ", level=" + level + ", power=" + power + '}' + '\n';
    }

    public void PrintRecu(Mage m, Set<Mage> apprentices, int depth) {
        for (int i = 0; i <= depth; i++)
            System.out.print("-");
        System.out.print(m);
        if (apprentices.size() == 0) {
            return;
        }
        /*
        for (int i = 0; i < apprentices.size(); i++)
            PrintRecu(apps[i], apps[i].apprentices, depth+1);
         */
        for (Mage apps : apprentices) {
            PrintRecu(apps, apps.getApprentices(), depth+1);
        }
        return;
    }

    public void PrintAll() {
        PrintRecu(this, this.getApprentices(), 0);
    }

    @Override
    public int compareTo(Object o) {
        if (mode == 1)
            return comparator.compare(this, (Mage) o);
        return Comparator.comparing(Mage::getName)
                .thenComparing(Mage::getLevel)
                .thenComparing(Mage::getPower)
                .compare(this, (Mage) o);
    }
}

//equals
//hashCode
//toString