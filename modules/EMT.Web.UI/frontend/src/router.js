import { browserHistory } from 'react-router'
import { Router } from 'react-router'

import MainLayout from './layouts/main/'
import Details from './pages/detailedView/'

import DashboardPage from './pages/dashboardPage/'
import SeparateChartsPage from './pages/separateChartsPage/'
import CompareChartsPage from './pages/compareChartsPage/'
import ChartsDemo from './pages/chartD3Demo'

const routeConfig = {
    path: '/',
    component: MainLayout,
    indexRoute: {
        component: DashboardPage
    },
    childRoutes: [
        {
            path: 'details',
            component: Details,
            childRoutes: []
        },
        {
            path: 'compare',
            component: CompareChartsPage,
            childRoutes: []
        },
        {
            path: 'demo',
            component: ChartsDemo,
            childRoutes: []
        },
        {
            path: 'charts',
            component: SeparateChartsPage,
            childRoutes: []
        }
    ]
}

const Rout = () => (
    <Router
        history={browserHistory}
        routes={routeConfig}
    />
)

export default Rout
