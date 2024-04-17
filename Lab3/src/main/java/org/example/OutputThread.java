package org.example;

import java.io.IOException;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;
import java.net.Socket;

public class OutputThread implements Runnable {
    private final Socket socket;

    ObjectInputStream in;

    public OutputThread(Socket socket) throws IOException {
        this.socket = socket;
        in = new ObjectInputStream(socket.getInputStream());
    }

    @Override
    public void run() {
        try {
            String response = (String) in.readObject();
            System.out.println(response);
        } catch (ClassNotFoundException e) {
            throw new RuntimeException(e);
        } catch (IOException e) {
            throw new RuntimeException(e);
        }
    }
}
