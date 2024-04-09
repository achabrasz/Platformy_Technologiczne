package org.example;

import java.util.Optional;
import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.Mockito.*;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;

public class EntityRepositoryTest {
    private EntityRepository repository;

    @BeforeEach
    void setUp() {
        repository = new EntityRepository();
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
