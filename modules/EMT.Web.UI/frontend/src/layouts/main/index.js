import Sidebar from '../../components/sidebar/'
import Header from '../../components/header/'

export default props => (
    <div className="container-main height-all">
        <Sidebar />
        <div className="content">
            <Header />
            <div className="content-main">
                {props.children}
            </div>
        </div>
    </div>
)
