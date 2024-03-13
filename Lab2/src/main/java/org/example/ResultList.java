package org.example;

import java.util.ArrayList;
import java.util.List;

public class ResultList {
    private List<Result> results = new ArrayList<>();

    public synchronized void Print() {
        for (Result result : results) {
            System.out.println(result);
        }
        results.clear();
    }

    public synchronized void addResult(Result result) {
        results.add(result);
    }
}

