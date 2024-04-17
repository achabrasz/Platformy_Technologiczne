package org.example;
import org.hibernate.Session;
import org.hibernate.SessionFactory;
import org.hibernate.boot.Metadata;
import org.hibernate.boot.MetadataSources;
import org.hibernate.boot.registry.StandardServiceRegistry;
import org.hibernate.boot.registry.StandardServiceRegistryBuilder;

import java.util.List;

public class Main {
    public static void main(String[] args) {
        StandardServiceRegistry standardRegistry = new StandardServiceRegistryBuilder()
                .configure("hibernate.cfg.xml")
                .build();
        Metadata metadata = new MetadataSources(standardRegistry)
                .getMetadataBuilder()
                .build();
        SessionFactory sessionFactory = metadata.getSessionFactoryBuilder().build();

        Session session = sessionFactory.openSession();
        try {
            session.beginTransaction();

            Tower tower1 = new Tower("Tower One", 100);

            Tower tower2 = new Tower("Tower Two", 150);

            Mage mage1 = new Mage("Mage One", 10, tower1);

            Mage mage2 = new Mage("Mage Two", 20, tower1);

            Mage mage3 = new Mage("Mage Three", 16, tower2);

            session.save(tower1);
            session.save(tower2);
            session.save(mage1);
            session.save(mage2);
            session.save(mage3);

            showAll(session, true);

            session.remove(mage1);

            showAll(session, true);

            session.save(mage1);

            List<Mage> magesWithHigherLevel = session.createQuery("FROM Mage WHERE level > :level", Mage.class)
                    .setParameter("level", 15)
                    .getResultList();
            System.out.println("Mages with level higher than 15:");
            magesWithHigherLevel.forEach(mage -> System.out.println(mage.getName() + " " + mage.getLevel()));

            List<Tower> towersWithLowerHeight = session.createQuery("FROM Tower WHERE height < :height", Tower.class)
                    .setParameter("height", 120)
                    .getResultList();
            System.out.println("\nTowers with height lower than 120:");
            towersWithLowerHeight.forEach(tower -> System.out.println(tower.getName() + " " + tower.getHeight()));

            List<Mage> magesFromTowerWithHigherLevel = session.createQuery("FROM Mage WHERE tower = :tower AND level > :level", Mage.class)
                    .setParameter("tower", tower1)
                    .setParameter("level", 15)
                    .getResultList();
            System.out.println("\nMages from Tower One with level higher than 15:");
            magesFromTowerWithHigherLevel.forEach(mage -> System.out.println(mage.getName() + " " + mage.getLevel()));

            session.getTransaction().commit();
        } catch (Exception ex) {
            ex.printStackTrace();
            session.getTransaction().rollback();
        } finally {
            session.close();
            sessionFactory.close();
        }
    }

    private static void showAll(Session session, Boolean showMages) {
        if (showMages) {
            List<Mage> allMages = session.createQuery("FROM Mage", Mage.class).getResultList();
            System.out.println("\nAll Mages:");
            allMages.forEach(mage -> System.out.println(mage.getName()));
        }
        else {
            List<Tower> allTowers = session.createQuery("FROM Tower", Tower.class).getResultList();
            System.out.println("\nAll Towers:");
            allTowers.forEach(tower -> System.out.println(tower.getName()));
        }
    }
}
