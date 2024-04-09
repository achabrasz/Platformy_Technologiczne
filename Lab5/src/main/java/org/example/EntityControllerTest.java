package org.example;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.Mockito.*;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;

import java.util.Optional;

public class EntityControllerTest {
    private EntityRepository repository;
    private EntityController controller;

    @BeforeEach
    void setUp() {
        repository = mock(EntityRepository.class);
        controller = new EntityController(repository);
    }

    @Test
    public void testDeleteNonExistingEntity() {
        EntityRepository repository = new EntityRepository();
        assertThrows(IllegalArgumentException.class, () -> repository.delete("nonExistingId"));
    }

    @Test
    public void testFindNonExistingEntity() {
        EntityRepository repository = new EntityRepository();
        Optional<Entity> entity = repository.find("nonExistingId");
        assertFalse(entity.isPresent());
    }

    @Test
    public void testFindExistingEntity() {
        EntityRepository repository = new EntityRepository();
        Entity entityToSave = new Entity("existingId", "EntityData");
        repository.save(entityToSave);
        Optional<Entity> entity = repository.find("existingId");
        assertTrue(entity.isPresent());
        assertEquals(entityToSave, entity.get());
    }

    @Test
    public void testSaveExistingEntity() {
        EntityRepository repository = new EntityRepository();
        Entity entityToSave = new Entity("existingId", "EntityData");
        repository.save(entityToSave);
        assertThrows(IllegalArgumentException.class, () -> repository.save(entityToSave));
    }
}
