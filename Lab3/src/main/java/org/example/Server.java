package org.example;

import java.io.IOException;
import java.io.ObjectInputStream;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.ArrayList;
import java.util.List;
import java.util.logging.Logger;

public class Server {
    private static final Logger logger = Logger.getLogger(Server.class.getName());
    private static final int PORT = 12345;

    private static final List<ClientHandler> clients = new ArrayList<>();
    private static final List<OutputThread> outputThreads = new ArrayList<>();

    public static void main(String[] args) {
        try (ServerSocket serverSocket = new ServerSocket(PORT)) {
            logger.info("Server started, listening on port " + PORT);

            while (true) {
                Socket clientSocket = serverSocket.accept();
                logger.info("New client connected: " + clientSocket);
                ClientHandler clientHandler = new ClientHandler(clientSocket);
                clients.add(clientHandler);
                //OutputThread outputThread = new OutputThread(clientSocket, clientHandler);
                //outputThreads.add(outputThread);
                new Thread(clientHandler).start();
            }
        } catch (IOException e) {
            logger.severe("Server exception: " + e.getMessage());
        }
    }
    public static void broadcastMessage(String message, ClientHandler sender) throws IOException {
        for (ClientHandler client : clients) {
            if (client != sender) {
                client.sendMessage(message);
            }
        }
    }
    public static void removeClient(ClientHandler clientHandler) {
        clients.remove(clientHandler);
    }

    public static void endServer() {
        for (ClientHandler client : clients) {
            try {
                client.sendMessage("Server is shutting down");
                client.sendMessage("end");
            } catch (IOException e) {
                e.printStackTrace();
            }
        }
        System.exit(0);
    }
}

