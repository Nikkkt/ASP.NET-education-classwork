document.addEventListener('DOMContentLoaded', () => {
    loadCart()
})

function loadCart() {
    const container = document.getElementById("cart-container");
    if (!container) throw "#cart-container not found";

    container.innerHTML = ""
    const userId = container.getAttribute("data-user-id");

    fetch("/api/cart?id=" + userId)
        .then(r => r.json())
        .then(j => {
            html = `
            <div class="row mx-5">
                <div class="col col-sm-12 col-lg-10 col-xl-8">
            `
            let totalCount = 0
            if (j.data == null || j.data.cartProducts.length == 0) {
                html = "Кошик порожній"
            } else {
                let total = 0
                for (let cartProduct of j.data.cartProducts) {
                    html += `
                    <div data-cart-id="${j.data.id}" class="row my-2 cart-product-item" data-cp-id="${cartProduct.id}">
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
                            <div class="text-center mx-auto cart-product-sum" >
                                <b>₴ <span data-role="cart-product-sum" data-price="${cartProduct.product.price}">${cartProduct.count * cartProduct.product.price}</span></b>
                            </div>
                        </div>
                    </div>
                    `
                    total += cartProduct.count * cartProduct.product.price
                    totalCount += cartProduct.count
                }

                if (totalCount > 0) {
                    html += `
                        <div class="d-flex align-items-center justify-content-center my-2">
                            <b>Всього <span data-role="cart-total-count">${totalCount}</span> товари(ів) на суму <span data-role="cart-total">${total}</span> грн</b>
                        </div>
                    `
                }
            }

            html += `
            <div class="d-flex justify-content-left">
                <a href="/Shop/Index" class="btn btn-info" style="margin-right: 10px;">До магазину</a>
                ${totalCount > 0 ? '<button onclick="buyClick()" class="btn btn-success">Перейти до оплати</button>' : ''}
            </div>
            `

            html += '</div></div>'

            container.innerHTML = html;
        });
}

function buyClick() {
    const block = document.querySelector('[data-cart-id]')
    const cartId = block.getAttribute('data-cart-id')
    const total = document.querySelector('[data-role="cart-total"]').innerText

    if (confirm("Чи підтверджуєте ви покупку на суму " + total)) {
        console.log(cartId)
        fetch("/api/cart?cartId=" + cartId, {
            method: "DELETE"
        }).then(r => r.json()).then(j => {
            console.log(j)
            if (j.data == "Deleted") {
                loadCart()
            } else {
                alert(j.data)
            }
        })
    }
}

function updateTotal() {
    let total = 0
    let totalCount = 0
    for (let s of document.querySelectorAll('[data-role="cart-product-sum"]')) {
        total += Number(s.innerHTML)
    }
    for (let c of document.querySelectorAll('[data-role="cart-product-count"]')) {
        totalCount += Number(c.innerHTML)
    }
    document.querySelector('[data-role="cart-total"]').innerHTML = total
    document.querySelector('[data-role="cart-total-count"]').innerHTML = totalCount
}

function decrementClick(e) {
    updateCart(e, -1)
}

function incrementClick(e) {
    updateCart(e, 1)
}

function updateCart(e, increment) {
    const parentBlock = e.target.closest("[data-cp-id]")
    const cpId = parentBlock.getAttribute("data-cp-id")
    fetch(`/api/cart?increment=${increment}&cpId=${cpId}`, {
        method: 'PUT'
    }).then(r => r.json()).then(j => {
        if (j.meta.count == 0) {
            loadCart()
            return
        }
        const countBlock = parentBlock.querySelector('[data-role="cart-product-count"]')
        countBlock.innerHTML = j.meta.count
        const sumBlock = parentBlock.querySelector('[data-role="cart-product-sum"]')
        sumBlock.innerHTML = j.meta.count * sumBlock.getAttribute('data-price')
        updateTotal()
    })
}