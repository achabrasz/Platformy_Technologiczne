package org.example;

import java.io.IOException;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;
import java.net.Socket;
import java.util.Scanner;
import java.util.logging.Logger;

public class Client {
    private static final Logger logger = Logger.getLogger(Client.class.getName());
    private static final String SERVER_ADDRESS = "localhost";
    private static final int PORT = 12345;

    public static void main(String[] args) {
        try (Socket socket = new Socket(SERVER_ADDRESS, PORT);
             ObjectOutputStream out = new ObjectOutputStream(socket.getOutputStream());
             ObjectInputStream in = new ObjectInputStream(socket.getInputStream());
             Scanner scanner = new Scanner(System.in)) {

            logger.info("Connected to server.");
            /*
            System.out.println("Enter a number of messages: ");
            int n = scanner.nextInt();
            for (int i = 0; i < n; i++) {
                Message message = new Message();
                message.setNumber(i + 1);
                System.out.println("Enter message " + (i + 1) + ": ");
                message.setText(scanner.next());
                out.writeObject(message);
            }
            */
            String text;
            while (true) {
                System.out.print("Enter message: ");
                String messageText = scanner.nextLine();

                Message message = new Message();
                message.setText(messageText);
                out.writeObject(message);

                String response = (String) in.readObject();
                System.out.println(response);
                if(messageText.equals("end") || response.equals("Server is shutting down")) {
                    System.exit(0);
                }
            }

        } catch (IOException e) {
            logger.severe("Client exception: " + e.getMessage());
        } catch (ClassNotFoundException e) {
            throw new RuntimeException(e);
        }
    }
}

