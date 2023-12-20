/**
 * plugins/vuetify.ts
 *
 * Framework documentation: https://vuetifyjs.com`
 */

// Styles
import '@mdi/font/css/materialdesignicons.css'
import 'vuetify/styles'

// Composables
import { createVuetify, ThemeDefinition } from 'vuetify'
import { md3 } from 'vuetify/blueprints'

// https://vuetifyjs.com/en/introduction/why-vuetify/#feature-guides
export default createVuetify({
  theme: {
    themes: {
      light: {
        colors: {
          profile: "#E8EAF6",
          homecard: "#EDE7F6",
        },
      },
      dark: {
        colors: {
          profile: "#263238",
          homecard: "#424242",
        }
      }
    },
  },
  blueprint: md3,
})
