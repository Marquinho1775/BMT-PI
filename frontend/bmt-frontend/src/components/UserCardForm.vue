<template>
    <v-main>
        <v-container>
            <v-card>
                <v-card-title>Información de la Tarjeta de Crédito</v-card-title>
                <v-card-text>
                    <v-form ref="form" v-model="isValid" lazy-validation>
                        <v-text-field v-model="creditCard.name" label="Nombre en la tarjeta" required></v-text-field>

                        <v-text-field v-model="creditCard.number" label="Número de tarjeta (XXXX-XXXX-XXXX-XXXX)"
                            :rules="[cardNumberRule]" required maxlength="19"></v-text-field>

                        <v-text-field v-model="creditCard.magicNumber" label="Código de seguridad (CVV)"
                            :rules="[cvvRule]" required maxlength="3" type="number"></v-text-field>

                        <v-text-field v-model="creditCard.dateVenc" label="Fecha de vencimiento (MM/AA)"
                            :rules="[expirationDateRule]" required maxlength="5"></v-text-field>
                    </v-form>
                </v-card-text>
                <v-card-actions>
                    <v-btn color="primary" @click="handleBack">Volver</v-btn>
                    <v-spacer></v-spacer>
                    <v-btn color="primary" :disabled="!isValid" @click="submitForm">Guardar</v-btn>
                </v-card-actions>
            </v-card>
        </v-container>
    </v-main>
</template>

<script>
import { ref, onMounted } from 'vue';
import axios from 'axios';
import { API_URL } from '@/main';

export default {
    setup() {
        const creditCard = ref({
            id: '',
            userID: '',
            name: '',
            number: '',
            magicNumber: '',
            dateVenc: ''
        });

        const isValid = ref(false);

        // Al montar el componente, obtenemos el userID del localStorage
        onMounted(() => {
            const user = JSON.parse(localStorage.getItem('user'));
            if (user && user.id) {
                creditCard.value.userID = user.id;
            }
        });

        // Reglas de validación
        const cardNumberRule = value =>
            /^(\d{4}-){3}\d{4}$/.test(value) || 'Formato inválido. Use XXXX-XXXX-XXXX-XXXX';
        const cvvRule = value =>
            /^\d{3}$/.test(value) || 'El CVV debe ser un número de 3 dígitos';
        const expirationDateRule = value =>
            /^\d{2}\/\d{2}$/.test(value) || 'Formato inválido. Use MM/AA';

        const handleBack = () => {
            this.$router.push('/profile');
        };

        const submitForm = () => {
            axios.post(`${API_URL}/CreditCard`, creditCard.value)
                .then(() => {
                    handleBack();
                })
                .catch((error) => {
                    console.log(error);
                });
        };

        return {
            creditCard,
            isValid,
            cardNumberRule,
            cvvRule,
            expirationDateRule,
            handleBack,
            submitForm
        };
    }
};
</script>

<style lang="scss" scoped></style>
