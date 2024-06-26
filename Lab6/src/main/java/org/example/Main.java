package org.example;

import java.io.IOException;
import java.nio.file.Path;
import java.util.concurrent.ForkJoinPool;

public class Main {
    public static void main(String[] args) throws IOException {

        if (args.length != 3) {
            System.err.println("inputDirectory outputDirectory threadPoolSize");
            System.exit(1);
        }

        long time = System.currentTimeMillis();

        Path inputDir = Path.of(args[0]);
        Path outputDir = Path.of(args[1]);
        int threadPoolSize = Integer.parseInt(args[2]);

        ForkJoinPool customThreadPool = new ForkJoinPool(threadPoolSize);

        customThreadPool.submit(() -> {
            try {
                ImageProcessor.main(new String[]{inputDir.toString(), outputDir.toString()});
            } catch (IOException e) {
                e.printStackTrace();
            }
        });

        customThreadPool.shutdown();
        System.out.println(System.currentTimeMillis() - time);

    }
}
