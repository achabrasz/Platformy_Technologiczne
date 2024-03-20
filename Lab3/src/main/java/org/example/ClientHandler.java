package org.example;

import java.io.IOException;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;
import java.net.Socket;
import java.util.logging.Logger;

public class ClientHandler implements Runnable {
    private static final Logger logger = Logger.getLogger(ClientHandler.class.getName());
    private final Socket clientSocket;

    private ObjectOutputStream out;

    public ClientHandler(Socket socket) {
        this.clientSocket = socket;
    }

    @Override
    public void run() {
        try (
                ObjectOutputStream out = new ObjectOutputStream(clientSocket.getOutputStream());
                ObjectInputStream in = new ObjectInputStream(clientSocket.getInputStream());
        ) {
            this.out = out;
            logger.info("Client connected from: " + clientSocket.getInetAddress());

            while (true) {
                Message message = (Message) in.readObject();
                logger.info("Message received: " + message.getText());
                Server.broadcastMessage("Client " + clientSocket.getInetAddress() + " said: " + message.getText(), this);
            }
        } catch (IOException | ClassNotFoundException e) {
            logger.severe("Exception handling client: " + e.getMessage());
        } finally {
            try {
                clientSocket.close();
                Server.removeClient(this);
                Server.endServer();
            } catch (IOException e) {
                logger.severe("Exception closing client socket: " + e.getMessage());
            }
        }
    }
    public void sendMessage(String message) throws IOException {
        if (out != null) {
            out.writeObject(message);
            out.flush();
        }
    }
}

