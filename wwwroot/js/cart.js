document.addEventListener('DOMContentLoaded', () => {
    loadCart()
})

function loadCart() {
    const container = document.getElementById("cart-container");
    if (!container) throw "#cart-container not found";
    const userId = container.getAttribute("data-user-id");

    fetch("/api/cart?id=" + userId)
        .then(r => r.json())
        .then(j => {
            let html = "";
            if (j.data == null || j.data.cartProducts == null) {
                html = "Кошик порожній";
            }
            else {
                html = `
                <div class="row mx-5">
                    <div class="col col-8">
                `;
                for (let cartProduct of j.data.cartProducts) {
                    html += `
                    <div class="row my-2 cart-product-item" data-product-id="${cartProduct.id}">
                        <div class="col col-2">
                            <a href="/Shop/Product/${cartProduct.product.slug ?? cartProduct.product.id}" class="a-no-underline">
                                <img src="/Home/Download/Shop_${cartProduct.product.picture}" alt="Picture"/>
                            </a>
                        </div>
                        <div class="col col-8 cart-product-info">
                            <h4>${cartProduct.product.name}</h4>
                            <p class="text-muted">${cartProduct.product.description}</p>
                        </div>
                        <div class="col col-2 cart-product-calc">
                            <div class="d-flex justify-content-between align-items-center">
                                <button onclick="decrementClick(event)" class="btn btn-outline-secondary">-</button>
                                <b data-role="cart-product-count">${cartProduct.count}</b>
                                <button onclick="incrementClick(event)" class="btn btn-outline-secondary">+</button>
                            </div>
                            <div class="text-center mx-auto cart-product-sum" data-role="cart-product-sum">
                                <b>₴ <span>${cartProduct.count * cartProduct.product.price}</span></b>
                            </div>
                        </div>
                    </div>
                    `;
                }
                html += '</div></div>';
            }
            container.innerHTML = html;
        });
}

function decrementClick(e) {
    const parentBlock = e.target.closest("[data-product-id]")
    const productId = parentBlock.getAttribute("data-product-id")
    console.log(productId + " decrement")
}

function incrementClick(e) {
    const parentBlock = e.target.closest("[data-product-id]")
    const productId = parentBlock.getAttribute("data-product-id")
    console.log(productId + " increment")
}