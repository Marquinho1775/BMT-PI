// Styles
import '@mdi/font/css/materialdesignicons.css'
import 'vuetify/styles'
import DayJsAdapter from '@date-io/dayjs'

// Vuetify
import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components';
import * as directives from 'vuetify/directives';

export default createVuetify({
  components,
  directives,
  theme: {
    defaultTheme: 'light',
  },
  date: {
    adapter: DayJsAdapter,
  },
});