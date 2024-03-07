package org.example;

import java.util.LinkedList;
import java.util.Queue;

public class TaskQueue {
    private Queue<Task> tasks = new LinkedList<>();

    public synchronized void addTask(Task task) {
        tasks.add(task);
        notifyAll();
    }

    public synchronized Task getTask() throws InterruptedException {
        while (tasks.isEmpty()) {
            wait();
        }
        return tasks.poll();
    }
}

