<template>
    <v-app class="d-flex flex-column">
        <AppHeader />
        <v-main class="flex-grow-1">
            <v-container>
                <v-card class="pa-5 mb-4">
                    <v-row>
                        <v-col cols="12" sm="12" md="8" class="mx-auto">
                            <h3 class="text-center">Editar Información de Perfil</h3>
                            <v-form ref="form" @submit.prevent="updateProfile" v-model="formValid">
                                <v-text-field
                                    label="Nombre de Usuario"
                                    v-model="profileData.username"
                                    placeholder="Ingrese el nuevo nombre de usuario"
                                    :rules="[profileData.username ? validateUsername : null]"
                                    :error-messages="usernameError"
                                    clearable
                                ></v-text-field>

                                <v-text-field
                                    label="Nombre"
                                    v-model="profileData.name"
                                    placeholder="Ingrese el nuevo nombre"
                                    :rules="[profileData.name ? validateName : null]"
                                    :error-messages="nameError"
                                    clearable
                                ></v-text-field>

                                <v-text-field
                                    label="Apellido"
                                    v-model="profileData.lastName"
                                    placeholder="Ingrese el nuevo apellido"
                                    :rules="[profileData.lastName ? validateLastName : null]"
                                    :error-messages="lastNameError"
                                    clearable
                                ></v-text-field>

                                <v-text-field
                                    label="Contraseña Anterior"
                                    v-model="oldPassword"
                                    :type="showPassword ? 'text' : 'password'"
                                    placeholder="Ingrese la contraseña actual"
                                    :append-icon="showPassword ? 'mdi-eye' : 'mdi-eye-off'"
                                    @click:append="showPassword = !showPassword"
                                    clearable
                                ></v-text-field>

                                <v-text-field
                                    label="Nueva Contraseña"
                                    v-model="profileData.password"
                                    :type="showNewPassword ? 'text' : 'password'"
                                    placeholder="Ingrese la nueva contraseña"
                                    :append-icon="showNewPassword ? 'mdi-eye' : 'mdi-eye-off'"
                                    @click:append="showNewPassword = !showNewPassword"
                                    :rules="[profileData.password ? validatePassword : null]"
                                    :error-messages="passwordError"
                                    clearable
                                ></v-text-field>

                                <v-text-field
                                    label="Confirmar Nueva Contraseña"
                                    v-model="confirmPassword"
                                    :type="showConfirmPassword ? 'text' : 'password'"
                                    placeholder="Confirme la nueva contraseña"
                                    :append-icon="showConfirmPassword ? 'mdi-eye' : 'mdi-eye-off'"
                                    @click:append="showConfirmPassword = !showConfirmPassword"
                                    :error-messages="confirmPasswordError"
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
            profileData: {
                username: '',
                name: '',
                lastName: '',
                password: ''
            },
            oldPassword: '',
            confirmPassword: '',
            showPassword: false,
            showNewPassword: false,
            showConfirmPassword: false,
            formValid: true,
            usernameError: '',
            nameError: '',
            lastNameError: '',
            passwordError: '',
            confirmPasswordError: ''
        };
    },
    computed: {
        isFormFilled() {
            return (
                this.profileData.username ||
                this.profileData.name ||
                this.profileData.lastName ||
                this.profileData.password
            );
        }
    },
    mounted() {
        const user = JSON.parse(localStorage.getItem('user'));
        if (user) {
            this.profileData = { ...user };
        }
    },
    methods: {
        async updateProfile() {
            if (this.profileData.password && this.profileData.password !== this.confirmPassword) {
                this.confirmPasswordError = 'Las contraseñas no coinciden';
                return;
            }

            try {
                let user = JSON.parse(localStorage.getItem('user')) || {};

                this.userId = user.id;

                const dataToUpdate = {
                    id: this.userId,
                    username: this.profileData.username || user.username,
                    name: this.profileData.name || user.name,
                    lastName: this.profileData.lastName || user.lastName,
                    password: this.profileData.password || undefined,
                };

                console.log("Datos a enviar:", dataToUpdate);

                const response = await axios.put(`${API_URL}/User/UpdateUser`, dataToUpdate);

                if (dataToUpdate.username) user.username = dataToUpdate.username;
                if (dataToUpdate.name) user.name = dataToUpdate.name;
                if (dataToUpdate.lastName) user.lastName = dataToUpdate.lastName;
                if (dataToUpdate.password) user.password = dataToUpdate.password;

                localStorage.setItem('user', JSON.stringify(user));

                this.profileData = { ...user };

                console.log(response);
                await this.$swal.fire({
                    title: 'Actualización exitosa',
                    text: '¡La información de su perfil ha sido actualizada correctamente!',
                    icon: 'success',
                    confirmButtonText: 'Ok',
                });

                this.$router.push('/profile');
            } catch (error) {
                console.error('Error al actualizar el perfil:', error);
                this.$swal.fire({
                    title: 'Error',
                    text: 'Hubo un error al actualizar su perfil. Inténtelo de nuevo.',
                    icon: 'error',
                    confirmButtonText: 'Ok',
                });
            }
        },
        goBack() {
            this.$router.push('/profile');
        },
        validateName(value) {
            const regex = /^[a-zA-ZáéíóúÁÉÍÓÚñÑ ]{1,20}$/;
            return regex.test(value) || 'El nombre solo puede contener letras y tener máximo 20 caracteres.';
        },
        validateLastName(value) {
            const regex = /^[a-zA-ZáéíóúÁÉÍÓÚñÑ ]{1,50}$/;
            return regex.test(value) || 'El apellido solo puede contener letras y tener máximo 50 caracteres.';
        },
        validateUsername(value) {
            const regex = /^[a-zA-Z0-9-_´.]{1,20}$/;
            return regex.test(value) || 'El nombre de usuario solo puede contener letras, números, y los caracteres especiales -_´.';
        },
        validatePassword(value) {
            const regex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{1,16}$/;
            return regex.test(value) || 'La contraseña debe incluir mayúsculas, minúsculas, números y un carácter especial.';
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
