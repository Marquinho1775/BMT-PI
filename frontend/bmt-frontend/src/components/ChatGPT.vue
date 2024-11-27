<template>
    <v-container>
        <!-- Botón Flotante -->
        <v-btn color="primary" @click="dialog = true" elevation="2">
            Hablar con GPT
        </v-btn>

        <v-dialog v-model="dialog" persistent max-width="600px">
            <v-card>
                <v-card-title>
                    <span class="text-h5">Chat con GPT</span>
                    <v-spacer></v-spacer>
                    <v-btn icon @click="dialog = false">
                        <v-icon>mdi-close</v-icon>
                    </v-btn>
                </v-card-title>

                <v-card-text class="chat-container">
                    <v-list>
                        <v-list-item v-for="(msg, index) in messages" :key="index" :class="{
                            'user-message': msg.role === 'user',
                            'bot-message': msg.role !== 'user'
                        }">
                            <v-list-item-content>
                                <v-list-item-title>{{ msg.content }}</v-list-item-title>
                                <v-list-item-subtitle>{{ msg.role === 'user' ? 'Tú' : 'Bot' }}</v-list-item-subtitle>
                            </v-list-item-content>
                        </v-list-item>
                    </v-list>
                </v-card-text>

                <v-card-actions>
                    <v-text-field v-model="input" label="Escribe tu mensaje..." @keyup.enter="handleSendMessage"
                        outlined dense class="flex-grow-1"></v-text-field>
                    <v-btn color="primary" @click="handleSendMessage">Enviar</v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </v-container>
</template>

<script>
import axios from "axios";

export default {
    name: "ChatGPT",
    data() {
        return {
            dialog: false,
            messages: [],
            input: "",
        };
    },
    methods: {
        async handleSendMessage() {
            if (this.input.trim() === "") return;

            const userMessage = { role: "user", content: this.input };
            this.messages.push(userMessage);

            try {
                const response = await axios.post("http://localhost:5000/api/chat", {
                    messages: this.messages,
                });

                const botMessage = response.data.choices[0].message;
                this.messages.push(botMessage);
            } catch (error) {
                console.error(
                    "Error al enviar el mensaje:",
                    error.response?.data || error.message
                );
                // Opcional: Mostrar una notificación al usuario
            }

            this.input = "";
            // Scroll automático hacia el último mensaje
            this.$nextTick(() => {
                const container = this.$el.querySelector(".chat-container");
                if (container) {
                    container.scrollTop = container.scrollHeight;
                }
            });
        },
    },
};
</script>

<style scoped>
.floating-button {
    position: fixed;
    bottom: 20px;
    right: 20px;
    z-index: 1000;
}

.chat-container {
    max-height: 400px;
    overflow-y: auto;
}

.user-message {
    justify-content: flex-end;
}

.bot-message {
    justify-content: flex-start;
}

.user-message .v-list-item-content {
    background-color: #e3f2fd;
    border-radius: 10px;
    padding: 10px;
}

.bot-message .v-list-item-content {
    background-color: #f1f1f1;
    border-radius: 10px;
    padding: 10px;
}
</style>