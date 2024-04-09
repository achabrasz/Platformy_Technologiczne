package org.example;

public class Entity {
    private String id;
    private String data;
    // Dodaj pozostałe pola encji i metody dostępowe

    public Entity(String id, String data) {
        this.id = id;
        this.data = data;
    }

    public String getId() {
        return id;
    }
}
