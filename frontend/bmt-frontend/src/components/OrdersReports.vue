<template>
    <v-card class="pending-orders-report">
        <!-- Botones y dropdown -->
        <v-row align="center" justify="space-between">
            <v-col cols="12" sm="4">
                <!-- Botón para seleccionar fecha de inicio -->
                <v-btn color="primary" text @click="startDateDialog = true">
                    Seleccionar Fecha de Inicio
                </v-btn>
                <span v-if="startDate">Fecha de Inicio: {{ formattedStartDate }}</span>

                <!-- Diálogo para la fecha de inicio -->
                <v-dialog v-model="startDateDialog" max-width="380px">
                    <v-card>
                        <v-card-title>
                            <span class="text-h6">Selecciona una Fecha</span>
                        </v-card-title>

                        <v-card-text>
                            <v-date-picker v-model="tempStartDate" color="secondary" locale="es">
                            </v-date-picker>
                        </v-card-text>
                        <v-card-actions>
                            <!-- Botones de acción -->
                            <v-spacer></v-spacer>
                            <v-btn text color="primary" @click="startDateDialog = false">Cancelar</v-btn>
                            <v-btn text color="primary" @click="saveStartDate">OK</v-btn>
                        </v-card-actions>
                    </v-card>
                </v-dialog>
            </v-col>

            <v-col cols="12" sm="4">
                <!-- Botón para seleccionar fecha final -->
                <v-btn color="primary" text @click="endDateDialog = true">
                    Seleccionar Fecha Final
                </v-btn>
                <span v-if="endDate">Fecha Final: {{ formattedEndDate }}</span>

                <!-- Diálogo para la fecha final -->
                <v-dialog v-model="endDateDialog" max-width="380px">
                    <v-card>
                        <v-card-title>
                            <span class="text-h6">Selecciona una Fecha</span>
                        </v-card-title>
                        <v-card-text>
                            <v-date-picker v-model="tempEndDate" color="secondary" locale="es">
                            </v-date-picker>
                        </v-card-text>
                        <v-card-actions>
                            <!-- Botones de acción -->
                            <v-spacer></v-spacer>
                            <v-btn text color="primary" @click="endDateDialog = false">Cancelar</v-btn>
                            <v-btn text color="primary" @click="saveEndDate">OK</v-btn>
                        </v-card-actions>
                    </v-card>
                </v-dialog>
            </v-col>

            <v-col cols="12" sm="3">
                <!-- Dropdown con opciones según el rol del usuario -->
                <v-select v-model="selectedOption" :items="dropdownOptions" label="Seleccione una opción"></v-select>
            </v-col>
        </v-row>

        <!-- Botón para generar reporte -->
        <v-btn color="primary" @click="generateReport">
            Generar Reporte
        </v-btn>

        <!-- Tabla con los datos del reporte -->
        <reports-table v-if="reportData.length"
            :titles="Type === 1 ? tableTitles1 : Type === 2 ? tableTitles2 : tableTitles3" :reports="reportData" />
    </v-card>
</template>

<script>
import axios from 'axios';
import { API_URL } from '@/main.js';

export default {
    name: 'OrdersReport',
    props: {
        Type: {
            type: Number,
            required: true,
        }
        // 1: Reporte de pedidos pendientes
        // 2: Reporte de pedidos entregados
        // 3: Reporte de pedidos cancelados
    },
    data() {
        return {
            // Fechas
            startDate: null,
            endDate: null,
            tempStartDate: null,
            tempEndDate: null,
            startDateDialog: false,
            endDateDialog: false,

            // Opción seleccionada en el dropdown
            selectedOption: null,

            // Opciones del dropdown
            dropdownOptions: [],

            // Datos del reporte
            reportData: [],

            // Usuario actual
            user: JSON.parse(localStorage.getItem('user') || '{}'),

            // Lista de emprendimientos (empresas)
            enterprises: [],

            tableTitles1: [
                'Número de orden',
                'Emprendimientos asociados',
                'Cantidad de ítems en la compra',
                'Fecha de creación',
                'Fecha de envío',
                'Estado',
                'Costo total de los ítems',
                'Costo de envío',
                'Costo total de la compra',
            ],

            tableTitles2: [
                'Número de orden',
                'Emprendimientos asociados',
                'Cantidad de ítems en la compra',
                'Fecha de creación',
                'Fecha de envío',
                'Fecha de recibido',
                'Costo total de los ítems',
                'Costo de envío',
                'Costo total de la compra',
            ],

            tableTitles3: [
                'Número de orden',
                'Emprendimientos asociados',
                'Cantidad de ítems en la compra',
                'Fecha de creación',
                'Fecha de cancelación',
                'Cancelado por',
                'Costo total de los ítems',
                'Costo de envío',
                'Costo total de la compra',
            ],
        };
    },
    computed: {
        formattedStartDate() {
            return this.startDate ? new Date(this.startDate).toLocaleDateString() : '';
        },
        formattedEndDate() {
            return this.endDate ? new Date(this.endDate).toLocaleDateString() : '';
        },
    },
    created() {
        this.initializeDropdownOptions();
    },
    methods: {
        async GetEnterprisesOfEntrepreneur() {
            try {
                const user = this.user;
                const obtainEntrepreneurResponse = await axios.get(API_URL + '/Entrepreneur/GetEntrepreneurByUserId?id=' + user.id);
                const entrepreneurId = obtainEntrepreneurResponse.data.identification;
                const enterprisesResponse = await axios.get(API_URL + '/Entrepreneur/my-registered-enterprises?Identification=' + entrepreneurId);
                this.enterprises = enterprisesResponse.data.data;
            } catch (error) {
                console.error('Error al obtener las empresas:', error);
                if (error.response) {
                    console.error('Datos de la respuesta del servidor:', error.response.data);
                }
            }
        },
        async initializeDropdownOptions() {
            if (this.user && this.user.role) {
                const userRole = this.user.role;
                if (userRole === 'cli') {
                    this.dropdownOptions = ['Mis pedidos'];
                } else if (userRole === 'emp') {
                    // Obtener las empresas asociadas
                    await this.GetEnterprisesOfEntrepreneur();
                    this.dropdownOptions = ['Mis pedidos', ...this.enterprises.map(e => e.name)];
                } else if (userRole === 'dev') {
                    // Obtener lista de todas las empresas
                    const response = await axios.get(API_URL + '/Enterprise');
                    const allEnterprises = response.data.data;
                    this.enterprises = allEnterprises;
                    this.dropdownOptions = ['Todos los pedidos', ...allEnterprises.map(e => e.name)];
                }
            } else {
                console.error('El usuario no está definido o no tiene un rol asignado.');
                // Manejar el error apropiadamente
            }
        },
        saveStartDate() {
            this.startDate = this.tempStartDate;
            this.startDateDialog = false;
        },
        saveEndDate() {
            this.endDate = this.tempEndDate;
            this.endDateDialog = false;
        },
        async generateReport() {
            // Validar que se hayan seleccionado las fechas y la opción
            if (!this.startDate || !this.endDate || !this.selectedOption) {
                alert('Por favor, seleccione las fechas y una opción antes de generar el reporte.');
                return;
            }

            // Construir el cuerpo de la solicitud
            let requestBody = {};

            if (this.Type == 1) {
                requestBody = {
                    fechaInicio: this.startDate,
                    fechaFin: this.endDate,
                    statusInicial: 0,
                    statusFinal: 3,
                };
            } else if (this.Type == 2) {
                requestBody = {
                    fechaInicio: this.startDate,
                    fechaFin: this.endDate,
                    statusInicial: 4,
                    statusFinal: 4,
                };
            } else if (this.Type == 3) {
                requestBody = {
                    fechaInicio: this.startDate,
                    fechaFin: this.endDate,
                    statusInicial: 5,
                    statusFinal: 6,
                };
            }

            // Lógica para userId y enterpriseId
            if (this.selectedOption === 'Mis pedidos') {
                requestBody.userId = this.user.id;
            } else if (this.selectedOption === 'Todos los pedidos') {
                // No se envía userId ni enterpriseId
            } else {
                const enterprise = this.enterprises.find(
                    e => e.name === this.selectedOption
                );
                if (enterprise) {
                    requestBody.enterpriseId = enterprise.id;
                } else {
                    console.error('Empresa no encontrada.');
                    return;
                }
            }

            console.log('Solicitud de reporte:', requestBody);

            try {
                const response = await axios.post(API_URL + '/Order/OrderReports', requestBody);
                const data = response.data;
                console.log('Datos del reporte:', data);

                if (this.Type == 1) {

                    // Procesar los datos para adaptarlos a la tabla
                    this.reportData = data.map(item => ({
                        numOrder: item.numOrder,
                        enterprises: item.enterprises,
                        itemsCount: item.itemsCount,
                        dateOfCreation: item.dateOfCreation,
                        dateOfDelivery: item.dateOfDelivery,
                        status: item.status,
                        productCost: item.productCost,
                        feeCost: item.feeCost,
                        totalCost: item.totalCost,
                    }));
                }
                else if (this.Type == 2) {
                    // Procesar los datos para adaptarlos a la tabla
                    this.reportData = data.map(item => ({
                        numOrder: item.numOrder,
                        enterprises: item.enterprises,
                        itemsCount: item.itemsCount,
                        dateOfCreation: item.dateOfCreation,
                        dateOfDelivery: item.dateOfDelivery,
                        dateReceived: item.DateReceived,
                        productCost: item.productCost,
                        feeCost: item.feeCost,
                        totalCost: item.totalCost,
                    }));
                }
                else if (this.Type == 3) {
                    // Procesar los datos para adaptarlos a la tabla
                    this.reportData = data.map(item => ({
                        numOrder: item.numOrder,
                        enterprises: item.enterprises,
                        itemsCount: item.itemsCount,
                        dateOfCreation: item.dateOfCreation,
                        dateOfCancelation: item.dateOfCancelation,
                        canceledBy: item.canceledBy,
                        productCost: item.productCost,
                        feeCost: item.feeCost,
                        totalCost: item.totalCost,
                    }));
                }
            } catch (error) {
                console.error('Error al generar el reporte:', error);
            }
        },
    },
};
</script>

<style scoped>
.pending-orders-report {
    padding: 20px;
}

.v-row {
    margin-bottom: 20px;
}

.v-btn {
    margin-top: 10px;
}

.reports-table {
    margin-top: 30px;
}
</style>