package org.example;

import org.apache.commons.lang3.tuple.Pair;

import javax.imageio.ImageIO;
import java.awt.*;
import java.awt.image.BufferedImage;
import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.util.List;
import java.util.concurrent.ForkJoinPool;
import java.util.stream.Collectors;

public class ImageProcessor {

    public static void main(String[] args) throws IOException {

        if (args.length != 2) {
            System.err.println("inputDirectory outputDirectory");
            System.exit(1);
        }
        long time = System.currentTimeMillis();
        Path inputDir = Path.of(args[0]);
        Path outputDir = Path.of(args[1]);

        List<Pair<String, BufferedImage>> images = loadImages(inputDir);
        processImages(images, outputDir);
        System.out.println(System.currentTimeMillis() - time);

    }

    private static List<Pair<String, BufferedImage>> loadImages(Path inputDir) throws IOException {
        List<Path> files;
        try (var stream = Files.list(inputDir)) {
            files = stream.collect(Collectors.toList());
        }

        return files.parallelStream()
                .map(path -> {
                    try {
                        return Pair.of(path.getFileName().toString(), ImageIO.read(path.toFile()));
                    } catch (IOException e) {
                        e.printStackTrace();
                        return null;
                    }
                })
                .collect(Collectors.toList());
    }

    private static void processImages(List<Pair<String, BufferedImage>> images, Path outputDir) {
        images.parallelStream().forEach(pair -> {
            BufferedImage original = pair.getRight();
            BufferedImage transformed = transformImage(original);
            String fileName = pair.getLeft();
            saveImage(transformed, outputDir.resolve(fileName));
        });
    }

    private static BufferedImage transformImage(BufferedImage original) {
        BufferedImage image = new BufferedImage(original.getWidth(), original.getHeight(), original.getType());
        for (int i = 0; i < original.getWidth(); i++) {
            for (int j = 0; j < original.getHeight(); j++) {
                int rgb = original.getRGB(i, j);
                Color color = new Color(rgb);
                int red = color.getRed();
                int blue = color.getBlue();
                int green = color.getGreen();
                Color outColor = new Color(red, blue, green);
                int outRgb = outColor.getRGB();
                image.setRGB(i, j, outRgb);
            } }
        return image;
    }

    private static void saveImage(BufferedImage image, Path outputPath) {
        try {
            ImageIO.write(image, "jpg", outputPath.toFile());
        } catch (IOException e) {
            e.printStackTrace();
        }
    }
}

