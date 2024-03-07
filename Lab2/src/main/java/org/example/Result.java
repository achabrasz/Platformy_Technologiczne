package org.example;

public class Result {
    private int number;
    private boolean isPrime;

    public Result(int number, boolean isPrime) {
        this.number = number;
        this.isPrime = isPrime;
    }

    @Override
    public String toString() {
        return "Number: " + number + ", is prime: " + isPrime + "\n";
    }
}
