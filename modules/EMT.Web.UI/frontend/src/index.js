import './index.scss'

import { Provider } from 'react-redux'
import store from './store'

import AppRouter from './router'

ReactDOM.render(
    <Provider store={store}>
        <AppRouter />
    </Provider>
    , document.getElementById('appRoot')
)
