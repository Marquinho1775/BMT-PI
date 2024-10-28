<template>
    <v-app class="d-flex flex-column">
        <AppHeader />
        <v-main class="flex-grow-1">
            <v-container>
                <v-card class="pa-5 mb-4">
                    <v-row>
                        <v-col cols="12" sm="12" md="8" class="mx-auto">
                            <h3 class="text-center">Editar Información del Emprendimiento</h3>
                            <v-form ref="form" @submit.prevent="updateEnterprise" v-model="formValid">
                                <v-text-field
                                    label="Nombre del Emprendimiento"
                                    v-model="enterpriseData.name"
                                    placeholder="Ingrese el nuevo nombre del emprendimiento"
                                    :rules="[enterpriseData.name ? validateName : null]"
                                    :error-messages="nameError"
                                    clearable
                                ></v-text-field>

                                <v-text-field
                                    label="Descripción"
                                    v-model="enterpriseData.description"
                                    placeholder="Ingrese la nueva descripción"
                                    :rules="[enterpriseData.description ? validateDescription : null]"
                                    :error-messages="descriptionError"
                                    clearable
                                ></v-text-field>

                                <v-text-field
                                    label="Correo Empresarial"
                                    v-model="enterpriseData.email"
                                    placeholder="Ingrese el nuevo correo empresarial"
                                    :rules="[enterpriseData.email ? validateEmail : null]"
                                    :error-messages="emailError"
                                    clearable
                                ></v-text-field>

                                <v-text-field
                                    label="Número de Teléfono"
                                    v-model="enterpriseData.phoneNumber"
                                    placeholder="Ingrese el nuevo número de teléfono"
                                    :rules="[enterpriseData.phoneNumber ? validatePhoneNumber : null]"
                                    :error-messages="phoneNumberError"
                                    clearable
                                ></v-text-field>

                                <div class="d-flex justify-content-between">
                                    <v-btn color="secondary" @click="goBack">Volver</v-btn>
                                    <v-btn type="submit" color="primary" :disabled="!formValid || !isFormFilled">
                                        Guardar Cambios
                                    </v-btn>
                                </div>
                            </v-form>
                        </v-col>
                    </v-row>
                </v-card>
            </v-container>
        </v-main>
        <AppFooter />
        <AppSidebar />
    </v-app>
</template>

<script>
import axios from 'axios';
import { API_URL } from '@/main.js';

export default {
    data() {
        return {
            enterpriseData: {
                name: '',
                description: '',
                email: '',
                phoneNumber: ''
            },
            formValid: true,
            nameError: '',
            descriptionError: '',
            emailError: '',
            phoneNumberError: ''
        };
    },
    computed: {
        isFormFilled() {
            return (
                this.enterpriseData.name ||
                this.enterpriseData.description ||
                this.enterpriseData.email ||
                this.enterpriseData.phoneNumber
            );
        }
    },
    async created() {
        const enterpriseId = this.$route.params.id;
        try {
            const response = await axios.get(`${API_URL}/Enterprise/${enterpriseId}`);
            this.enterpriseData = { ...response.data };
        } catch (error) {
            console.error("Error al cargar la información del emprendimiento:", error);
        }
    },
    methods: {
        async updateEnterprise() {
            try {
                const dataToUpdate = {
                    id: this.$route.params.id,
                    ...(this.enterpriseData.name ? { name: this.enterpriseData.name } : {}),
                    ...(this.enterpriseData.description ? { description: this.enterpriseData.description } : {}),
                    ...(this.enterpriseData.email ? { email: this.enterpriseData.email } : {}),
                    ...(this.enterpriseData.phoneNumber ? { phoneNumber: this.enterpriseData.phoneNumber } : {})
                };

                console.log("Datos a actualizar:", dataToUpdate);
                const response = await axios.put(`${API_URL}/Enterprise/${dataToUpdate.id}`, dataToUpdate);
                await this.$swal.fire({
                    title: 'Actualización exitosa',
                    text: '¡La información del emprendimiento ha sido actualizada correctamente!',
                    icon: 'success',
                    confirmButtonText: 'Ok',
                });
                console.log(response);

                this.$router.push(`/enterprise/${dataToUpdate.id}`);
            } catch (error) {
                console.error('Error al actualizar el emprendimiento:', error);
                this.$swal.fire({
                    title: 'Error',
                    text: 'Hubo un error al actualizar el emprendimiento. Inténtelo de nuevo.',
                    icon: 'error',
                    confirmButtonText: 'Ok',
                });
            }
        },
        goBack() {
            this.$router.push(`/enterprise/${this.$route.params.id}`);
        },
        validateName(value) {
            return value.length <= 100 || 'El nombre debe tener máximo 100 caracteres.';
        },
        validateDescription(value) {
            return value.length <= 255 || 'La descripción debe tener máximo 255 caracteres.';
        },
        validateEmail(value) {
            const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            return regex.test(value) || 'Ingrese un correo electrónico válido. (Formato: Xxx@xxx.xxx)';
        },
        validatePhoneNumber(value) {
            const regex = /^[0-9]{8}$/;
            return regex.test(value) || 'Ingrese un número de teléfono válido de 8 dígitos.';
        }
    }
};
</script>

<style scoped>
.v-app-bar {
    background-color: #9FC9FC;
}
.v-footer {
    height: 50px;
    background-color: #9FC9FC;
}
.v-card {
    background-color: #ffffff;
}
.text-center {
    text-align: center;
}
</style>
