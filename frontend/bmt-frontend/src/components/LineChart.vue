<template>
  <v-card>
    <v-card-title class="text-h6">Gastos Mensuales por Tarifas de Entrega</v-card-title>
    <v-card-text>
      <div class="chart-container">
        <Line
          id="line-chart-id"
          :data="chartData"
          :options="chartOptions"
        />
      </div>
    </v-card-text>
  </v-card>
</template>

<script>
import { Line } from 'vue-chartjs'
import {
  Chart as ChartJS,
  Title,
  Tooltip,
  Legend,
  LineElement,
  PointElement,
  CategoryScale,
  LinearScale,
} from 'chart.js'

ChartJS.register(
  Title,
  Tooltip,
  Legend,
  LineElement,
  PointElement,
  CategoryScale,
  LinearScale
)

export default {
  name: 'LineChart',
  components: { Line },
  props: {
    dataset: {
      type: Array,
      required: true,
    },
  },
  data() {
    return {
      labels: [
        'Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun',
        'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'
      ],
      chartOptions: {
        responsive: true,
        maintainAspectRatio: false,
        scales: {
          x: {
            title: {
              display: true,
              text: 'Mes',
            },
          },
          y: {
            title: {
              display: true,
              text: 'Gastos (colones)',
            },
            beginAtZero: true,
          },
        },
        plugins: {
          legend: {
            position: 'top',
            labels: {
              font: {
                size: 12,
              },
            },
          },
          tooltip: {
            mode: 'index',
            intersect: false,
            callbacks: {
              label: function (context) {
                const label = context.dataset.label || ''
                const value = context.parsed.y || 0
                return `${label}: ${value} colones`
              },
            },
          },
        },
      },
    }
  },
  computed: {
    chartData() {
      return {
        labels: this.labels,
        datasets: [
          {
            label: 'Gastos por Tarifas de Entrega',
            data: this.dataset,
            fill: false,
            borderColor: 'rgba(75, 192, 192, 1)',
            tension: 0.1,
          },
        ],
      }
    },
  },
}
</script>

<style scoped>
.v-card {
  margin-top: 20px;
}

.chart-container {
  width: 100%;
  overflow-x: auto;
  height: 400px;
}

.chart-container ::v-deep canvas {
  min-width: 600px;
}

@media (max-width: 600px) {
  .chart-container {
    height: 300px;
  }
  .chart-container ::v-deep canvas {
    min-width: 500px;
  }
  .v-card-title {
    font-size: 16px;
  }
}
</style>
