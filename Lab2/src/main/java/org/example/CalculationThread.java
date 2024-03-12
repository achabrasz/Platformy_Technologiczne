package org.example;

import static java.lang.Math.random;
import static java.lang.Math.sqrt;

public class CalculationThread implements Runnable{
    private TaskQueue taskQueue;
    private ResultList resultList;

    public CalculationThread(TaskQueue taskQueue, ResultList resultList) {
        this.taskQueue = taskQueue;
        this.resultList = resultList;
    }

    public void simulateLongCalculation() throws InterruptedException {
        int r = (int)(random()*2);
        if (r == 0) {
            toSleep();
        }
    }

    public void toSleep() throws InterruptedException {
        Thread.sleep(1000);
    }

    @Override
    public void run() {
        while (true) {
            try {
                Task task = taskQueue.getTask();
                Result result = calculateResult(task);
                resultList.addResult(result);
                resultList.Print();
            } catch (InterruptedException e) {
                break;
            }
        }
    }

    private Result calculateResult(Task task) throws InterruptedException {
        int number = task.getNumber();
        boolean isPrime = true;
        for (int i = 2; i < sqrt(number); i++) {
            if (number % i == 0) {
                isPrime = false;
                break;
            }

        }
        simulateLongCalculation();
        return new Result(number, isPrime);
    }
}
