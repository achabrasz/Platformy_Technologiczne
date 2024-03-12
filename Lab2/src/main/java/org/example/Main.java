package org.example;

import java.util.Scanner;
import java.lang.Thread;

public class Main {
    public static void main(String[] args) {
        TaskQueue taskQueue = new TaskQueue();
        ResultList resultList = new ResultList();
        int numberOfThreads = Integer.parseInt(args[0]);
        Thread[] threads = new Thread[numberOfThreads];
        for (int i = 0; i < numberOfThreads; i++) {
            threads[i] = new Thread(new CalculationThread(taskQueue, resultList));
            threads[i].start();
        }
        Scanner scanner = new Scanner(System.in);
        while (true) {
            System.out.println("Enter a number or type exit to finish program: ");
            String input = scanner.nextLine();
            if (input.equals("exit")) {
                for (Thread thread : threads) {
                    try {
                        thread.interrupt();
                    } catch (Exception e) {
                        e.printStackTrace();
                    }
                }
                break;
            }
            try {
                Integer.parseInt(input);
            } catch (NumberFormatException e) {
                System.out.println("Please enter a valid number in the range of int");
                continue;
            }
            int number = Integer.parseInt(input);
            taskQueue.addTask(new Task(number));
        }
        for (Thread thread : threads) {
            try {
                thread.join();
            } catch (Exception e) {
                e.printStackTrace();
            }
        }
        //resultList.Print();
        System.exit(0);
    }
}