package org.example;

import java.util.Optional;

public class EntityController {
    private EntityRepository repository;

    public EntityController(EntityRepository repository) {
        this.repository = repository;
    }

    public String find(String id) {
        Optional<Entity> optionalEntity = repository.find(id);
        if (optionalEntity.isPresent()) {
            return "Found entity: " + optionalEntity.get().getId();
        } else {
            return "Entity not found";
        }
    }

    public String delete(String id) {
        try {
            repository.delete(id);
            return "done";
        } catch (IllegalArgumentException e) {
            return "not found";
        }
    }

    public String save(String id) {
        try {
            repository.save(new Entity("existingId", id));
            return "done";
        } catch (IllegalArgumentException e) {
            return "bad request";
        }
    }
}
