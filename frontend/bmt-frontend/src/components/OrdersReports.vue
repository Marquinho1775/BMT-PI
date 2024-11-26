<template>
    <v-card class="pending-orders-report">
        <v-row justify="center" class="mb-6">
            <h2 class="report-title" v-if="Type === 1">Reporte de Pedidos Pendientes</h2>
            <h2 class="report-title" v-else-if="Type === 2">Reporte de Pedidos Entregados</h2>
            <h2 class="report-title" v-else-if="Type === 3">Reporte de Pedidos Cancelados</h2>
        </v-row>

        <v-row align="center" justify="flex-start">
            <v-col cols="auto" class="mr-4">
                <v-card-actions>
                    <v-btn color="success" text @click="startDateDialog = true" class="date-button" variant="tonal">
                        <v-icon left>mdi-calendar</v-icon>
                        {{ startDate ? formattedStartDate : 'Seleccionar Fecha de Inicio' }}
                    </v-btn>
                </v-card-actions>

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

            <v-col cols="auto" class="mr-4">
                <v-card-actions>
                    <v-btn color="success" text @click="endDateDialog = true" class="date-button" variant="tonal">
                        <v-icon left>mdi-calendar</v-icon>
                        {{ endDate ? formattedEndDate : 'Seleccionar Fecha Final' }}
                    </v-btn>
                </v-card-actions>

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

            <v-col cols="auto">
                <v-card-actions>
                    <v-select v-model="selectedOption" :items="dropdownOptions" label="Seleccione una opción"
                        class="ml-4"></v-select>
                </v-card-actions>
            </v-col>
        </v-row>

        <!-- Botón para generar reporte -->
        <v-row justify="flex-start" class="mt-4 mb-6">
            <v-col cols="auto">
                <v-btn color="success" @click="generateReport">
                    Generar Reporte
                </v-btn>
            </v-col>
            <v-col cols="auto">
                <v-btn color="secondary" @click="exportToPDF" :disabled="!reportData.length">
                    Exportar a PDF
                </v-btn>
            </v-col>
        </v-row>

        <!-- Tabla con los datos del reporte -->
        <reports-table v-if="reportData.length"
            :titles="Type === 1 ? tableTitles1 : Type === 2 ? tableTitles2 : tableTitles3"
            :keys="Type === 1 ? tableKeys1 : Type === 2 ? tableKeys2 : tableKeys3" :reports="reportData" />
    </v-card>
    <v-divider></v-divider>
</template>

<script>
import axios from 'axios';
import jsPDF from "jspdf";
import "jspdf-autotable";
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
            tableKeys1: [
                'numOrder',
                'enterprises',
                'itemsCount',
                'dateOfCreation',
                'dateOfDelivery',
                'status',
                'productCost',
                'feeCost',
                'totalCost',
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
            tableKeys2: [
                'numOrder',
                'enterprises',
                'itemsCount',
                'dateOfCreation',
                'dateOfDelivery',
                'dateReceived',
                'productCost',
                'feeCost',
                'totalCost',
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
            tableKeys3: [
                'numOrder',
                'enterprises',
                'itemsCount',
                'dateOfCreation',
                'dateOfCancelation',
                'canceledBy',
                'productCost',
                'feeCost',
                'totalCost',
            ],
        };
    },
    computed: {
        formattedStartDate() {
            return this.startDate ? new Date(this.startDate).toLocaleDateString('es-ES') : '';
        },
        formattedEndDate() {
            return this.endDate ? new Date(this.endDate).toLocaleDateString('es-ES') : '';
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
                this.enterprises = enterprisesResponse.data.success;
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
                        dateReceived: item.dateReceived,
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
                        canceledBy: item.cancelBy,
                        productCost: item.productCost,
                        feeCost: item.feeCost,
                        totalCost: item.totalCost,
                    }));
                }
                console.log('Datos del reporte procesados:', this.reportData);
            } catch (error) {
                console.error('Error al generar el reporte:', error);
            }
        },
        exportToPDF() {
            const doc = new jsPDF({ orientation: "landscape" });
            let title = '';

            if (this.Type === 1) {
                title = 'Reporte de Pedidos Pendientes';
            } else if (this.Type === 2) {
                title = 'Reporte de Pedidos Entregados';
            } else if (this.Type === 3) {
                title = 'Reporte de Pedidos Cancelados';
            }

            doc.text(title, 20, 20);

            const tableData = this.reportData.map((row) =>
                Object.values(row)
            );

            const tableTitles = this.Type === 1 ? this.tableTitles1 : this.Type === 2 ? this.tableTitles2 : this.tableTitles3;

            doc.autoTable({
                head: [tableTitles],
                body: tableData,
                startY: 30,
                theme: "grid",
                headStyles: { fillColor: [169, 197, 255] },
            });

            doc.save(`${title.replace(/\s+/g, '_').toLowerCase()}.pdf`);
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

.report-title {
    font-size: 24px;
    font-weight: bold;
}

/* Estilos adicionales para los botones de fecha */
.date-button {
    display: flex;
    align-items: center;
}

.date-button .v-icon {
    margin-right: 8px;
}

/* Ajustes para el espaciado entre elementos alineados a la derecha */
.mr-4 {
    margin-right: 16px;
    /* Puedes ajustar el valor según tus necesidades */
}
</style>
