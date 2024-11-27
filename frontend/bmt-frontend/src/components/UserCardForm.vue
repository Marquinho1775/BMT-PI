<template>
    <v-main>
        <v-container>
            <!-- Tarjeta de crédito simulada -->
            <v-card class="credit-card mx-auto" width="100%" max-width="500" elevation="5" hover>
                <v-card-text>
                    <div class="card-chip">
                        <v-icon large>mdi-chip</v-icon>
                    </div>
                    <div class="card-number">
                        {{ formatCardNumber(creditCard.number) }}
                    </div>
                    <div class="card-details">
                        <div class="card-holder">
                            <span>Titular</span>
                            <div>{{ creditCard.name || 'NOMBRE DEL TITULAR' }}</div>
                        </div>
                        <div class="card-expiry">
                            <span>Vence</span>
                            <div>{{ creditCard.dateVenc || 'MM/AA' }}</div>
                        </div>
                    </div>
                </v-card-text>
            </v-card>

            <!-- Formulario para ingresar los datos -->
            <v-card>
                <v-card-title>Información de la Tarjeta de Crédito</v-card-title>
                <v-card-text>
                    <v-form ref="form" v-model="isValid" lazy-validation>
                        <v-text-field v-model="creditCard.name" label="Nombre en la tarjeta" required></v-text-field>

                        <v-text-field v-model="creditCard.number" label="Número de tarjeta (XXXX-XXXX-XXXX-XXXX)"
                            :rules="[cardNumberRule]" required maxlength="19"></v-text-field>

                        <v-text-field v-model="creditCard.magicNumber" label="Código de seguridad (CVV)"
                            :rules="[cvvRule]" required maxlength="3" type="number"></v-text-field>

                        <!-- Nueva sección para la fecha de vencimiento -->
                        <v-row>
                            <v-col cols="6">
                                <v-select v-model="dateMonth" :items="months" label="Mes" required
                                    :rules="[requiredRule]"></v-select>
                            </v-col>
                            <v-col cols="6">
                                <v-select v-model="dateYear" :items="years" label="Año" required
                                    :rules="[requiredRule]"></v-select>
                            </v-col>
                        </v-row>
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
import axios from 'axios';
import { API_URL } from '@/main';

export default {
    data() {
        return {
            creditCard: {
                id: '',
                userID: '',
                name: '',
                number: '',
                magicNumber: '',
                dateVenc: '',
            },
            isValid: false,
            dateMonth: null,
            dateYear: null,
            months: Array.from({ length: 12 }, (_, i) => (i + 1).toString().padStart(2, '0')),
            years: Array.from({ length: 7 }, (_, i) => (24 + i).toString().padStart(2, '0')),
            // Reglas de validación
            cardNumberRule: (value) =>
                /^(\d{4}-){3}\d{4}$/.test(value) || 'Formato inválido. Use XXXX-XXXX-XXXX-XXXX',
            cvvRule: (value) =>
                /^\d{3}$/.test(value) || 'El CVV debe ser un número de 3 dígitos',
            requiredRule: (value) => !!value || 'Este campo es requerido',
        };
    },
    mounted() {
        // Al montar el componente, obtenemos el userID del localStorage
        const user = JSON.parse(localStorage.getItem('user'));
        if (user && user.id) {
            this.creditCard.userID = user.id;
        }
    },
    watch: {
        dateMonth() {
            this.updateDateVenc();
        },
        dateYear() {
            this.updateDateVenc();
        },
    },
    methods: {
        updateDateVenc() {
            if (this.dateMonth && this.dateYear) {
                this.creditCard.dateVenc = `${this.dateMonth}/${this.dateYear}`;
            } else {
                this.creditCard.dateVenc = '';
            }
        },
        formatCardNumber(number) {
            // Enmascarar todos los dígitos excepto los últimos cuatro
            if (!number) return '•••• •••• •••• ••••';
            const digitsOnly = number.replace(/-/g, '').replace(/\s+/g, '');
            const maskedNumber = digitsOnly
                .replace(/\d(?=\d{4})/g, '•')
                .replace(/(.{4})/g, '$1 ')
                .trim();
            return maskedNumber;
        },
        handleBack() {
            this.$router.push('/profile');
        },
        submitForm() {
            axios
                .post(`${API_URL}/CreditCard`, this.creditCard)
                .then(() => {
                    // Mostrar alerta de éxito y navegar después de que el usuario haga clic en OK
                    this.$swal.fire({
                        title: '¡Tarjeta creada con éxito!',
                        icon: 'success',
                        confirmButtonText: 'OK',
                    }).then(() => {
                        this.handleBack();
                    });
                })
                .catch((error) => {
                    // Mostrar alerta de error
                    this.$swal.fire({
                        title: 'Error al crear la tarjeta',
                        text: error.response ? error.response.data.message : error.message,
                        icon: 'error',
                        confirmButtonText: 'OK',
                    });
                    console.log(error);
                });
        },
    },
};
</script>

<style scoped>
.credit-card {
    background: linear-gradient(135deg, #1e3c72 0%, #2a5298 100%);
    color: white;
    border-radius: 15px;
    position: relative;
    padding: 20px;
    margin-bottom: 20px;
}

.card-chip {
    position: absolute;
    top: 20px;
    left: 20px;
}

.card-number {
    margin-top: 60px;
    font-size: 24px;
    letter-spacing: 3px;
    text-align: center;
}

.card-details {
    display: flex;
    justify-content: space-between;
    margin-top: 40px;
}

.card-holder,
.card-expiry {
    font-size: 14px;
}

.card-holder span,
.card-expiry span {
    font-size: 12px;
    opacity: 0.8;
}

.card-holder div,
.card-expiry div {
    font-size: 16px;
    text-transform: uppercase;
}
</style>
