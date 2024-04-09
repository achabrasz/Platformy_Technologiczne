package org.example;

import java.util.HashMap;
import java.util.Map;
import java.util.Optional;

public class EntityRepository {
    private Map<String, Entity> collection;

    public EntityRepository() {
        this.collection = new HashMap<>();
    }

    public Optional<Entity> find(String id) {
        return Optional.ofNullable(collection.get(id));
    }

    public void delete(String id) {
        if (!collection.containsKey(id)) {
            throw new IllegalArgumentException("Entity with id " + id + " does not exist");
        }
        collection.remove(id);
    }

    public void save(Entity entity) {
        if (collection.containsKey(entity.getId())) {
            throw new IllegalArgumentException("Entity with id " + entity.getId() + " already exists");
        }
        collection.put(entity.getId(), entity);
    }
}