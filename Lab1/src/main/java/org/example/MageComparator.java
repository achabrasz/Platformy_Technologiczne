package main.java.org.example;

import java.util.Comparator;

public class MageComparator implements Comparator<Mage> {
    @Override
    public int compare(Mage o1, Mage o2) {
        return Comparator.comparing(Mage::getLevel)
                .thenComparing(Mage::getName)
                .thenComparing(Mage::getPower)
                .compare(o1, o2) * -1;
    }
}
